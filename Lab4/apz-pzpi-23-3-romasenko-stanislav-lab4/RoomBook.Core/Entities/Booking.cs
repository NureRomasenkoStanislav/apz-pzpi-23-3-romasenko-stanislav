namespace RoomBook.Core.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Purpose { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsNoShow { get; set; }
        public User User { get; set; } = null!;
        public Room Room { get; set; } = null!;
    }
}