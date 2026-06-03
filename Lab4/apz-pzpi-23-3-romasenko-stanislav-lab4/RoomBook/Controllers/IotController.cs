using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RoomBook.API.Hubs;

[ApiController]
[Route("api/[controller]")]
public class IotController : ControllerBase
{
    private readonly IHubContext<IotHub> _hubContext;

    public IotController(IHubContext<IotHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("unlock/{iotDeviceId}")]
    public async Task<IActionResult> ForceUnlock(string iotDeviceId)
    {
        await _hubContext.Clients.Group(iotDeviceId).SendAsync("ReceiveCommand", "Unlock");
        return Ok(new { message = $"Команду розблокування відправлено на пристрій {iotDeviceId}" });
    }
}