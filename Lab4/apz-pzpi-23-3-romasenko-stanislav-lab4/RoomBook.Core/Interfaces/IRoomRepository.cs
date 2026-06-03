using RoomBook.Core.Entities;

namespace RoomBook.Core.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room> CreateRoomAsync(Room room);
        Task<bool> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int roomId);
        Task<Room?> GetByIdAsync(int id);
    }
}