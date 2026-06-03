using Microsoft.AspNetCore.SignalR;
using RoomBook.Core.Interfaces;

namespace RoomBook.API.Hubs
{
    public class IotHub : Hub<IIotClient>
    {
        public async Task RegisterDevice(string iotDeviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, iotDeviceId);
        }
    }
}