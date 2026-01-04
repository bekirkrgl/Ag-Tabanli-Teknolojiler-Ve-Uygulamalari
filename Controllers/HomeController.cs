using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Models;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Services;

namespace HastaneRandevuSistemi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly NotificationService _notificationService;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, NotificationService notificationService)
    {
        _logger = logger;
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var specializations = await _context.Specializations.ToListAsync();
        var doctors = await _context.Doctors
            .Include(d => d.Specialization)
            .Where(d => d.IsActive)
            .Take(6)
            .ToListAsync();
        
        ViewBag.Specializations = specializations;
        ViewBag.Doctors = doctors;
        
        return View();
    }

    public async Task<IActionResult> Dashboard()
    {
        // Dashboard için istatistikleri al
        var doctorCount = await _context.Doctors.CountAsync(d => d.IsActive);
        var patientCount = await _context.Patients.CountAsync(p => p.IsActive);
        var appointmentCount = await _context.Appointments.CountAsync();
        var specializationCount = await _context.Specializations.CountAsync(s => s.IsActive);
        
        ViewBag.DoctorCount = doctorCount;
        ViewBag.PatientCount = patientCount;
        ViewBag.AppointmentCount = appointmentCount;
        ViewBag.SpecializationCount = specializationCount;
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // GET: Home/GetNotifications
    public async Task<IActionResult> GetNotifications()
    {
        try
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync();
            var unreadCount = await _notificationService.GetUnreadCountAsync();
            
            return Json(new
            {
                success = true,
                notifications = notifications.Select(n => new
                {
                    id = n.Id,
                    title = n.Title,
                    message = n.Message,
                    type = (int)n.Type,
                    isRead = n.IsRead,
                    createdDate = n.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")
                }),
                unreadCount = unreadCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return Json(new { success = false, message = "Bildirimler yüklenirken hata oluştu." });
        }
    }

    // POST: Home/MarkNotificationAsRead
    [HttpPost]
    public async Task<IActionResult> MarkNotificationAsRead([FromBody] int id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return Json(new { success = false, message = "Bildirim okundu olarak işaretlenirken hata oluştu." });
        }
    }

    // POST: Home/MarkAllNotificationsAsRead
    [HttpPost]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        try
        {
            await _notificationService.MarkAllAsReadAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return Json(new { success = false, message = "Tüm bildirimler okundu olarak işaretlenirken hata oluştu." });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
