using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;
using HastaneRandevuSistemi.Services;

namespace HastaneRandevuSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DoctorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DoctorAvailabilityService _availabilityService;

        public DoctorApiController(ApplicationDbContext context, DoctorAvailabilityService availabilityService)
        {
            _context = context;
            _availabilityService = availabilityService;
        }

        /// <summary>
        /// Tüm doktorları getirir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive)
                .ToListAsync();
            return Ok(doctors);
        }

        /// <summary>
        /// Belirli bir doktoru getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        /// <summary>
        /// Doktorun müsait tarihlerini getirir
        /// </summary>
        [HttpGet("{id}/available-dates")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableDates(int id, [FromQuery] int daysAhead = 30)
        {
            var availableDates = await _availabilityService.GetAvailableDatesAsync(id, daysAhead);
            return Ok(availableDates.Select(d => d.ToString("yyyy-MM-dd")));
        }

        /// <summary>
        /// Doktorun belirli bir tarih için müsait saatlerini getirir
        /// </summary>
        [HttpGet("{id}/available-time-slots")]
        public async Task<ActionResult<object>> GetAvailableTimeSlots(int id, [FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                return BadRequest(new { message = "Geçersiz tarih formatı" });
            }

            var availableSlots = await _availabilityService.GetAvailableTimeSlotsAsync(id, parsedDate);
            
            return Ok(new
            {
                success = true,
                slots = availableSlots.Select(s => new
                {
                    date = s.ToString("yyyy-MM-dd"),
                    time = s.ToString("HH:mm")
                })
            });
        }
    }
}

