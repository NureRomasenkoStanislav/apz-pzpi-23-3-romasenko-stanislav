namespace RoomBook.Core.Interfaces
{
    public interface IIotService
    {
        Task SendUnlockCommandAsync(string deviceId);
        Task SendLockCommandAsync(string deviceId);
        Task UnlockRoomAsync(int roomId);
    }
}