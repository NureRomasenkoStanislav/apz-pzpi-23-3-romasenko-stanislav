namespace RoomBook.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; 
        public bool IsActive { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}