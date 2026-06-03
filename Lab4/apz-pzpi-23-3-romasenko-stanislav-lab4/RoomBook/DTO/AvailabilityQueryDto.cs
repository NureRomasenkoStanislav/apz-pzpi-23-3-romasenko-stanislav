namespace RoomBook.API.DTOs
{
    public class BookingRequestDto
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Purpose { get; set; }
    }
}