using RoomBook.Core.Interfaces;
using RoomBook.Infrastructure.Data;
using RoomBook.Core.Entities;

namespace RoomBook.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly RoomBookDbContext _context;
        public AdminRepository(RoomBookDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int userId, string action, string details)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}