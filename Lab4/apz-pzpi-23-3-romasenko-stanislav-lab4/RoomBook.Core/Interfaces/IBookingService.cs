using RoomBook.Core.Entities;

namespace RoomBook.Core.Interfaces
{
    public interface IBookingService
    {
        Task<bool> ProcessBookingAsync(Booking booking);
    }
}