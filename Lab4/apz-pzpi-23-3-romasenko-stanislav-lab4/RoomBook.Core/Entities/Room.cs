namespace RoomBook.Core.Entities
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public TimeSpan WorkingHoursStart { get; set; }
        public TimeSpan WorkingHoursEnd { get; set; }
        public string? Description { get; set; }
        public bool IsArchived { get; set; }
        public string? IotDeviceId { get; set; }
        public bool IsLocked { get; set; } = true;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
    }
}