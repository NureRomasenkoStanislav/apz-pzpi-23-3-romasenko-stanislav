using Microsoft.EntityFrameworkCore;
using RoomBook.Core.Entities;

namespace RoomBook.Infrastructure.Data
{
    public class RoomBookDbContext : DbContext
    {
        public RoomBookDbContext(DbContextOptions<RoomBookDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<Equipment> Equipment { get; set; } = null!;
        public DbSet<RoomEquipment> RoomEquipment { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomEquipment>()
                .HasKey(re => new { re.RoomId, re.EquipmentId });

            modelBuilder.Entity<RoomEquipment>()
                .HasOne(re => re.Room)
                .WithMany(r => r.RoomEquipments)
                .HasForeignKey(re => re.RoomId);

            modelBuilder.Entity<RoomEquipment>()
                .HasOne(re => re.Equipment)
                .WithMany(e => e.RoomEquipments)
                .HasForeignKey(re => re.EquipmentId);

            base.OnModelCreating(modelBuilder);
        }
    }
}