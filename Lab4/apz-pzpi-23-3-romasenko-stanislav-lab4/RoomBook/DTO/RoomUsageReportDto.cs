namespace RoomBook.API.DTOs
{
    public class RoomUsageReportDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;

        public double TotalBookedHours { get; set; }

        public double UsagePercentage { get; set; }
    }
}