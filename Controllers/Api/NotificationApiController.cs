using Microsoft.AspNetCore.Mvc;
using HastaneRandevuSistemi.Services;

namespace HastaneRandevuSistemi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NotificationApiController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationApiController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Okunmamış bildirimleri getirir
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetNotifications()
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync();
            var unreadCount = await _notificationService.GetUnreadCountAsync();

            return Ok(new
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

        /// <summary>
        /// Bildirimi okundu olarak işaretler
        /// </summary>
        [HttpPost("{id}/mark-as-read")]
        public async Task<ActionResult<object>> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretler
        /// </summary>
        [HttpPost("mark-all-as-read")]
        public async Task<ActionResult<object>> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync();
            return Ok(new { success = true });
        }
    }
}

