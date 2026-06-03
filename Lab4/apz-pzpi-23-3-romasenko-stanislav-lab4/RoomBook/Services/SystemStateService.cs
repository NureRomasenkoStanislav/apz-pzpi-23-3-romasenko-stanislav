using RoomBook.Core.Interfaces;

namespace RoomBook.API.Services
{
    public class SystemStateService : ISystemStateService
    {
        public bool IsMaintenanceMode { get; set; } = false;
    }
}