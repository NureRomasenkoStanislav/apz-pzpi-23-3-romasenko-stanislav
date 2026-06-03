using RoomBook.Core.Entities;

namespace RoomBook.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string?> Authenticate(string email, string password);
        string GenerateJwtToken(User user);
    }
}