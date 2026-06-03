namespace RoomBook.API.DTOs
{
    public class RoomDto
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public TimeSpan WorkingHoursStart { get; set; }
        public TimeSpan WorkingHoursEnd { get; set; }
        public string? Description { get; set; }
    }
}