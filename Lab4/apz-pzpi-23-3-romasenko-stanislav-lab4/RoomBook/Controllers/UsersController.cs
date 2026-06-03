using Microsoft.AspNetCore.Mvc;
using RoomBook.API.DTOs;
using RoomBook.Core.Entities;
using RoomBook.Core.Interfaces;
using Microsoft.AspNetCore.Authorization; 


namespace RoomBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Користувач з ID {id} не знайдений.");
            }
            return Ok(user);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Password))
            {
                return BadRequest("Пароль є обов'язковим.");
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Користувач з таким Email вже існує.");
            }

            var userEntity = new User
            {
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = userDto.Role,
                IsActive = userDto.IsActive
            };

            var createdUser = await _userRepository.CreateUserAsync(userEntity);

            createdUser.PasswordHash = string.Empty;

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateDto userDto)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound($"Користувач з ID {id} не знайдений.");
            }

            existingUser.Email = userDto.Email;
            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.Role = userDto.Role; 
            existingUser.IsActive = userDto.IsActive;

            var result = await _userRepository.UpdateUserAsync(existingUser);

            if (!result)
            {
                return BadRequest("Помилка при оновленні користувача.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userRepository.DeleteUserAsync(id);

            if (!deleted)
            {
                return NotFound($"Користувач з ID {id} не знайдений.");
            }

            return NoContent();
        }
    }
}