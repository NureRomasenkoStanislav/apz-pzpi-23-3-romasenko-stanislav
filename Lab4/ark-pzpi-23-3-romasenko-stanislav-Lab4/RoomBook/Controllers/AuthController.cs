using Microsoft.AspNetCore.Mvc;
using RoomBook.API.DTOs;
using RoomBook.Core.Interfaces;

namespace RoomBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Email та пароль є обов'язковими.");
            }

            var token = await _authService.Authenticate(loginDto.Email, loginDto.Password);

            if (token == null)
            {
                return Unauthorized("Неправильний Email або пароль.");
            }

            return Ok(new { token = token });
        }
    }
}