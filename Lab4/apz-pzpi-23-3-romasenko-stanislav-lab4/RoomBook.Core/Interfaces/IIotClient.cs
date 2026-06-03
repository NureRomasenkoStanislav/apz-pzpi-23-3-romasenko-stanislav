namespace RoomBook.Core.Interfaces
{
    public interface IIotClient
    {
        Task ReceiveCommand(string command);
    }
}