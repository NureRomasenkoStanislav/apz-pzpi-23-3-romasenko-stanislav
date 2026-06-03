namespace RoomBook.Core.Interfaces
{
    public interface ISystemStateService
    {
        bool IsMaintenanceMode { get; set; }
    }
}