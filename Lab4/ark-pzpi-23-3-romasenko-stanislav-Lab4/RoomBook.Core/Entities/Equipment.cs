namespace RoomBook.Core.Entities
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
    }
}