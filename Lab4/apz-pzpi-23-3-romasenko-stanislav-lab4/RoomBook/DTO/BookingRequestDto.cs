namespace RoomBook.API.DTOs
{
    public class AvailabilityQueryDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MinCapacity { get; set; } = 1;
    }
}