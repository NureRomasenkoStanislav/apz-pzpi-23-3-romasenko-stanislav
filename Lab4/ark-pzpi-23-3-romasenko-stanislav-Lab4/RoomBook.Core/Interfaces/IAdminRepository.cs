namespace RoomBook.Core.Interfaces
{
    public interface IAdminRepository
    {
        Task LogActionAsync(int userId, string action, string details);
    }
}