namespace RoomBook.Core.Entities
{
    public class RoomEquipment
    {
        public int RoomId { get; set; }
        public int EquipmentId { get; set; }
        public Room Room { get; set; } = null!;
        public Equipment Equipment { get; set; } = null!;
    }
}