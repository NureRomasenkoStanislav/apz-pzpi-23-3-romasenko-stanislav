using Microsoft.EntityFrameworkCore;
using RoomBook.Core.Entities;
using RoomBook.Core.Interfaces;
using RoomBook.Infrastructure.Data;
using RoomBook.Core.DTOs;

namespace RoomBook.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly RoomBookDbContext _context;
        private readonly IIotService _iotService; 

        public BookingRepository(RoomBookDbContext context, IIotService iotService)
        {
            _context = context;
            _iotService = iotService;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime start, DateTime end, int minCapacity)
        {
            var suitableRooms = await _context.Rooms
               .Where(r => r.Capacity >= minCapacity && !r.IsArchived)
               .Select(r => r.RoomId)
               .ToListAsync();

            if (!suitableRooms.Any()) return Enumerable.Empty<Room>();

            var bookedRoomIds = await _context.Bookings
               .Where(b => suitableRooms.Contains(b.RoomId) &&
                           b.StartTime < end &&
                           b.EndTime > start)
               .Select(b => b.RoomId)
               .Distinct()
               .ToListAsync();

            return await _context.Rooms
                .Where(r => suitableRooms.Contains(r.RoomId) && !bookedRoomIds.Contains(r.RoomId))
                .ToListAsync();
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var room = await _context.Rooms.AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == booking.RoomId);

            if (room != null && !string.IsNullOrEmpty(room.IotDeviceId))
            {
                if (booking.StartTime <= DateTime.UtcNow && booking.EndTime > DateTime.UtcNow)
                {
                    await _iotService.SendUnlockCommandAsync(room.IotDeviceId);
                }
            }

            return booking;
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking is null) return false;

            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room != null && !string.IsNullOrEmpty(room.IotDeviceId))
            {
                await _iotService.SendLockCommandAsync(room.IotDeviceId);
            }

            _context.Bookings.Remove(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoomUsageReportDto>> GetUsageReportAsync(DateTime startDate, DateTime endDate)
        {
            double totalDays = (endDate - startDate).TotalDays;
            if (totalDays <= 0) totalDays = 1;

            var allRooms = await _context.Rooms.AsNoTracking().ToListAsync();
            var bookings = await _context.Bookings
                .Where(b => b.StartTime < endDate && b.EndTime > startDate)
                .AsNoTracking().ToListAsync();

            return allRooms.Select(room =>
            {
                var roomBookings = bookings.Where(b => b.RoomId == room.RoomId);
                double totalBookedMinutes = roomBookings.Sum(b =>
                {
                    var effectiveStart = b.StartTime > startDate ? b.StartTime : startDate;
                    var effectiveEnd = b.EndTime < endDate ? b.EndTime : endDate;
                    return effectiveEnd <= effectiveStart ? 0.0 : (effectiveEnd - effectiveStart).TotalMinutes;
                });

                double totalBookedHours = totalBookedMinutes / 60.0;
                double totalAvailableHours = totalDays * (room.WorkingHoursEnd - room.WorkingHoursStart).TotalHours;

                return new RoomUsageReportDto
                {
                    RoomId = room.RoomId,
                    RoomName = room.Name,
                    TotalBookedHours = Math.Round(totalBookedHours, 2),
                    UsagePercentage = totalAvailableHours > 0 ? Math.Round((totalBookedHours / totalAvailableHours) * 100.0, 2) : 0
                };
            }).OrderByDescending(r => r.UsagePercentage);
        }
        public async Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .ToListAsync();
        }
    }
}