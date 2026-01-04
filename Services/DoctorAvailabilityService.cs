using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;

namespace HastaneRandevuSistemi.Services
{
    public class DoctorAvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public DoctorAvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> GetAvailableDatesAsync(int doctorId, int daysAhead = 30)
        {
            var doctor = await _context.Doctors
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
                return new List<DateTime>();

            var availableDates = new List<DateTime>();
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(daysAhead);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (await IsDoctorAvailableOnDateAsync(doctorId, date))
                {
                    availableDates.Add(date);
                }
            }

            return availableDates;
        }

        public async Task<List<DateTime>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var doctor = await _context.Doctors
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
                return new List<DateTime>();

            var workingHour = doctor.WorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == date.DayOfWeek && wh.IsActive);

            if (workingHour == null)
                return new List<DateTime>();

            var timeSlots = new List<DateTime>();
            var startTime = workingHour.StartTime;
            var endTime = workingHour.EndTime;
            var slotDuration = TimeSpan.FromMinutes(30); // 30 dakikalık slotlar

            for (var time = startTime; time < endTime; time = time.Add(slotDuration))
            {
                var slotDateTime = date.Date.Add(time);
                
                if (await IsDoctorAvailableAtTimeAsync(doctorId, slotDateTime))
                {
                    timeSlots.Add(slotDateTime);
                }
            }

            return timeSlots;
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime dateTime)
        {
            var doctor = await _context.Doctors
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null || !doctor.IsActive)
                return false;

            // Çalışma saatleri kontrolü
            var workingHour = doctor.WorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == dateTime.DayOfWeek && wh.IsActive);

            if (workingHour == null)
                return false;

            var timeOfDay = dateTime.TimeOfDay;
            if (timeOfDay < workingHour.StartTime || timeOfDay >= workingHour.EndTime)
                return false;

            // Müsaitlik blokları kontrolü
            var isBlocked = doctor.AvailabilityBlocks
                .Any(ab => ab.IsActive && 
                          dateTime >= ab.StartDateTime && 
                          dateTime < ab.EndDateTime);

            if (isBlocked)
                return false;

            // Mevcut randevular kontrolü
            var hasAppointment = await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && 
                              a.AppointmentDate.Date == dateTime.Date &&
                              a.AppointmentTime == timeOfDay &&
                              a.Status != AppointmentStatus.Cancelled);

            return !hasAppointment;
        }

        private async Task<bool> IsDoctorAvailableOnDateAsync(int doctorId, DateTime date)
        {
            var doctor = await _context.Doctors
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null || !doctor.IsActive)
                return false;

            // Çalışma saatleri kontrolü
            var workingHour = doctor.WorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == date.DayOfWeek && wh.IsActive);

            if (workingHour == null)
                return false;

            // Müsaitlik blokları kontrolü
            var isBlocked = doctor.AvailabilityBlocks
                .Any(ab => ab.IsActive && 
                          date >= ab.StartDateTime.Date && 
                          date <= ab.EndDateTime.Date);

            return !isBlocked;
        }

        private async Task<bool> IsDoctorAvailableAtTimeAsync(int doctorId, DateTime dateTime)
        {
            var doctor = await _context.Doctors
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null || !doctor.IsActive)
                return false;

            // Çalışma saatleri kontrolü
            var workingHour = doctor.WorkingHours
                .FirstOrDefault(wh => wh.DayOfWeek == dateTime.DayOfWeek && wh.IsActive);

            if (workingHour == null)
                return false;

            var timeOfDay = dateTime.TimeOfDay;
            if (timeOfDay < workingHour.StartTime || timeOfDay >= workingHour.EndTime)
                return false;

            // Müsaitlik blokları kontrolü
            var isBlocked = doctor.AvailabilityBlocks
                .Any(ab => ab.IsActive && 
                          dateTime >= ab.StartDateTime && 
                          dateTime < ab.EndDateTime);

            if (isBlocked)
                return false;

            // Mevcut randevular kontrolü
            var hasAppointment = await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && 
                              a.AppointmentDate.Date == dateTime.Date &&
                              a.AppointmentTime == timeOfDay &&
                              a.Status != AppointmentStatus.Cancelled);

            return !hasAppointment;
        }
    }
}