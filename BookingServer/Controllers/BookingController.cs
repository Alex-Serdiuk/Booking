using BookingServer.Models;
using BookingServer.Models.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public BookingController(BookingDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                List<Booking> bookings = new List<Booking>();

                foreach (var roomId in request.RoomIds)
                {
                    var roomNumber = await _context.RoomNumbers
                         .Include(rn => rn.Room) // Include Room to access Price
                         .FirstOrDefaultAsync(rn => rn.Id == roomId);
                    if (roomNumber == null)
                    {
                        continue; // Skip or handle this case as per your logic
                    }

                    var booking = new Booking
                    {
                        User = user
                    };

                    foreach (var date in request.Dates)
                    {
                        booking.UnavailableDates.Add(new UnavailableDate
                        {
                            Date = date,
                            RoomNumber = roomNumber,
                            Price = roomNumber.Room.Price // Record price from Room
                        });
                    }

                    bookings.Add(booking);
                }

                _context.Bookings.AddRange(bookings);
                await _context.SaveChangesAsync();

                return Ok($"Booking(s) created successfully with IDs: {string.Join(", ", bookings.Select(b => b.Id))}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the booking: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.UnavailableDates) // Завантажуємо UnavailableDates разом з Booking
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound("Booking not found.");
                }

                _context.Bookings.Remove(booking); // Видаляємо booking, що включає UnavailableDates

                await _context.SaveChangesAsync();

                return Ok($"Booking with ID: {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the booking: " + ex.Message);
            }
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.User) // Включення деталей користувача
                    .Include(b => b.UnavailableDates) // Включення деталей недоступних дат
                        .ThenInclude(ud => ud.RoomNumber) // Включення номерів кімнат
                            .ThenInclude(rn => rn.Room) // Включення деталей кімнат
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving bookings: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.User) // Включення деталей користувача
                    .Include(b => b.UnavailableDates) // Включення деталей недоступних дат
                        .ThenInclude(ud => ud.RoomNumber) // Включення номерів кімнат
                            .ThenInclude(rn => rn.Room) // Включення деталей кімнат
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound($"Booking with ID: {id} not found.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the booking: " + ex.Message);
            }
        }

        [HttpGet("GetBookingsByUserId/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByUserId(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var bookings = await _context.Bookings
                    .Where(b => b.User.Id == userId)
                    .Include(b => b.User) // Включення деталей користувача
                    .Include(b => b.UnavailableDates) // Включення деталей недоступних дат
                        .ThenInclude(ud => ud.RoomNumber) // Включення номерів кімнат
                            .ThenInclude(rn => rn.Room) // Включення деталей кімнат
                                .ThenInclude(r => r.Hotel) // Включення деталей готелю
                                    .ThenInclude(h => h.HotelImages) // Включення зображень готелю
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving bookings by user ID: " + ex.Message);
            }
        }

        [HttpGet("GetBookingsByHotelId/{hotelId}")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByHotelId(int hotelId)
        {
            try
            {
                var hotel = await _context.Hotels
                    .Include(h => h.Rooms) // Включення номерів готелю
                        .ThenInclude(r => r.RoomNumbers) // Включення номерів кімнат
                            .ThenInclude(rn => rn.UnavailableDates) // Включення недоступних дат
                                .ThenInclude(ud => ud.Booking) // Включення бронювань
                    .FirstOrDefaultAsync(h => h.Id == hotelId);

                if (hotel == null)
                {
                    return NotFound("Hotel not found.");
                }

                // Збираємо всі бронювання для номерів готелю
                var bookings = hotel.Rooms
                    .SelectMany(r => r.RoomNumbers)
                    .SelectMany(rn => rn.UnavailableDates)
                    .Select(ud => ud.Booking)
                    .Distinct()
                    .ToList();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving bookings by hotel ID: " + ex.Message);
            }
        }
    }
}
