using RoomBook.Core.Entities;
using RoomBook.Core.DTOs; 


namespace RoomBook.Core.Interfaces
{
    public interface IBookingRepository
    {
       
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime start, DateTime end, int minCapacity);
        Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId);

        Task<Booking> CreateBookingAsync(Booking booking);

        Task<Booking?> GetBookingByIdAsync(int bookingId);

        Task<bool> UpdateBookingAsync(Booking booking);

        Task<bool> DeleteBookingAsync(int bookingId);

        Task<bool> AddBookingAsync(Booking booking);

        Task<IEnumerable<RoomUsageReportDto>> GetUsageReportAsync(DateTime startDate, DateTime endDate);
    }
}