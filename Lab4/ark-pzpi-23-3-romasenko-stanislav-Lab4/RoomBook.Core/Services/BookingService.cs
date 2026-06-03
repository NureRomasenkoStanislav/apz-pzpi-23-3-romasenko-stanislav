using RoomBook.Core.Entities;
using RoomBook.Core.Interfaces;

namespace RoomBook.Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IRoomRepository _roomRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IIotService _iotService;
        private readonly ISystemStateService _systemState; 

        public BookingService(
            IBookingRepository repository,
            IRoomRepository roomRepository,
            IAdminRepository adminRepository,
            IIotService iotService,
            ISystemStateService systemState) 
        {
            _repository = repository;
            _roomRepository = roomRepository;
            _adminRepository = adminRepository;
            _iotService = iotService;
            _systemState = systemState; 
        }

        public async Task<bool> ProcessBookingAsync(Booking booking)
        {
            if (_systemState.IsMaintenanceMode)
                throw new Exception("Система тимчасово заблокована для технічного обслуговування.");

            if (booking.StartTime < DateTime.UtcNow)
                throw new Exception("Бронювання не може бути у минулому.");

            if (booking.EndTime <= booking.StartTime)
                throw new Exception("Час завершення має бути пізніше часу початку.");

            var existingBookings = await _repository.GetByRoomIdAsync(booking.RoomId);
            foreach (var existing in existingBookings)
            {
                if (booking.StartTime < existing.EndTime && booking.EndTime > existing.StartTime)
                {
                    throw new Exception("Ця кімната вже зайнята на обраний час.");
                }
            }

            var success = await _repository.AddBookingAsync(booking);

            if (success)
            {
                await _adminRepository.LogActionAsync(booking.UserId, "CREATE_BOOKING", $"Бронювання кімнати {booking.RoomId}");

                if (booking.StartTime <= DateTime.UtcNow.AddMinutes(1))
                {
                    await _iotService.UnlockRoomAsync(booking.RoomId);
                }
            }

            return success;
        }
    }
}