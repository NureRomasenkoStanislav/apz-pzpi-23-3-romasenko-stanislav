using Microsoft.EntityFrameworkCore;
using RoomBook.Core.Entities;
using RoomBook.Core.Interfaces;
using RoomBook.Infrastructure.Data;

namespace RoomBook.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly RoomBookDbContext _context;

        public RoomRepository(RoomBookDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetRoomByIdAsync(int roomId)
        {
            return await _context.Rooms
                .Include(r => r.RoomEquipments)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.RoomEquipments)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }
        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<bool> UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room is null) return false;

            _context.Rooms.Remove(room);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}