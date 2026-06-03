using Microsoft.AspNetCore.SignalR;
using RoomBook.API.Hubs;
using RoomBook.Core.Interfaces;

namespace RoomBook.API.Services
{
    public class IotService : IIotService
    {
        private readonly IHubContext<IotHub> _hubContext;

        public IotService(IHubContext<IotHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendUnlockCommandAsync(string deviceId)
        {
            await _hubContext.Clients.Group(deviceId).SendAsync("ReceiveCommand", "Unlock");
        }

        public async Task SendLockCommandAsync(string deviceId)
        {
            await _hubContext.Clients.Group(deviceId).SendAsync("ReceiveCommand", "Lock");
        }
        public async Task UnlockRoomAsync(int roomId)
        {
            await Task.CompletedTask;
        }
    }
}