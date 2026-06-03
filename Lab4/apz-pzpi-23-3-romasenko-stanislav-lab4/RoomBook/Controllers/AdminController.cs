using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RoomBook.Core.Interfaces;
using System.Security.Claims;
using RoomBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using RoomBook.Core.Entities;

namespace RoomBook.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly RoomBookDbContext _context;
        private readonly ISystemStateService _systemState; 

        public AdminController(RoomBookDbContext context, ISystemStateService systemState)
        {
            _context = context;
            _systemState = systemState;
        }

        [HttpPost("maintenance")]
        public IActionResult ToggleMaintenance([FromBody] bool enabled)
        {
            _systemState.IsMaintenanceMode = enabled;
            return Ok(new { message = $"Режим обслуговування: {(enabled ? "Увімкнено" : "Вимкнено")}" });
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetLogs()
        {
            return Ok(await _context.AuditLogs
                .Include(l => l.User) 
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync());
        }

        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] string newRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var oldRole = user.Role;
            user.Role = newRole;

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int adminId = int.Parse(adminIdClaim ?? "0");

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = adminId,
                Action = "CHANGE_ROLE",
                Details = $"Змінено роль юзера {id} з {oldRole} на {newRole}",
                Timestamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Роль успішно оновлено" });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                TotalBookings = await _context.Bookings.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(),
                TopRoomId = await _context.Bookings
                    .GroupBy(b => b.RoomId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync()
            };
            return Ok(stats);
        }
    }
}