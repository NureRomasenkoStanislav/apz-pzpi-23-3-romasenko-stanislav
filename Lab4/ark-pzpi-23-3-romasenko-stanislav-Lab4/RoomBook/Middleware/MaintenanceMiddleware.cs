using RoomBook.Core.Interfaces;

namespace RoomBook.API.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISystemStateService systemState)
        {
            if (systemState.IsMaintenanceMode && !context.Request.Path.StartsWithSegments("/api/admin"))
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync("Система на технічному обслуговуванні.");
                return;
            }

            await _next(context);
        }
    }
}