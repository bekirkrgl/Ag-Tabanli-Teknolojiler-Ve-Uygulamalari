using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;

namespace HastaneRandevuSistemi.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string title, string message, NotificationType type, 
            int? doctorId = null, int? patientId = null, int? appointmentId = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentId = appointmentId,
                CreatedDate = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(int? doctorId = null, int? patientId = null)
        {
            var query = _context.Notifications
                .Where(n => n.IsActive && !n.IsRead);

            if (doctorId.HasValue)
            {
                query = query.Where(n => n.DoctorId == doctorId || n.DoctorId == null);
            }

            if (patientId.HasValue)
            {
                query = query.Where(n => n.PatientId == patientId || n.PatientId == null);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllNotificationsAsync(int? doctorId = null, int? patientId = null)
        {
            var query = _context.Notifications
                .Where(n => n.IsActive);

            if (doctorId.HasValue)
            {
                query = query.Where(n => n.DoctorId == doctorId || n.DoctorId == null);
            }

            if (patientId.HasValue)
            {
                query = query.Where(n => n.PatientId == patientId || n.PatientId == null);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int? doctorId = null, int? patientId = null)
        {
            var query = _context.Notifications
                .Where(n => n.IsActive && !n.IsRead);

            if (doctorId.HasValue)
            {
                query = query.Where(n => n.DoctorId == doctorId || n.DoctorId == null);
            }

            if (patientId.HasValue)
            {
                query = query.Where(n => n.PatientId == patientId || n.PatientId == null);
            }

            var notifications = await query.ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(int? doctorId = null, int? patientId = null)
        {
            var query = _context.Notifications
                .Where(n => n.IsActive && !n.IsRead);

            if (doctorId.HasValue)
            {
                query = query.Where(n => n.DoctorId == doctorId || n.DoctorId == null);
            }

            if (patientId.HasValue)
            {
                query = query.Where(n => n.PatientId == patientId || n.PatientId == null);
            }

            return await query.CountAsync();
        }

        // Appointment related notifications
        public async Task CreateAppointmentNotificationAsync(Appointment appointment, string action)
        {
            string title = "";
            string message = "";

            switch (action.ToLower())
            {
                case "created":
                    title = "Yeni Randevu Oluşturuldu";
                    message = $"{appointment.Patient?.FullName} adlı hasta için {appointment.AppointmentDate:dd.MM.yyyy} tarihinde {appointment.AppointmentTime:HH:mm} saatinde randevu oluşturuldu.";
                    break;
                case "updated":
                    title = "Randevu Güncellendi";
                    message = $"{appointment.Patient?.FullName} adlı hastanın randevusu güncellendi.";
                    break;
                case "cancelled":
                    title = "Randevu İptal Edildi";
                    message = $"{appointment.Patient?.FullName} adlı hastanın randevusu iptal edildi.";
                    break;
                case "completed":
                    title = "Randevu Tamamlandı";
                    message = $"{appointment.Patient?.FullName} adlı hastanın randevusu tamamlandı.";
                    break;
            }

            await CreateNotificationAsync(title, message, NotificationType.Appointment, 
                appointment.DoctorId, appointment.PatientId, appointment.Id);
        }
    }
}
