using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace HastaneRandevuSistemi.Services
{
    public class DummyEmailSender : IEmailSender
    {
        private readonly ILogger<DummyEmailSender> _logger;

        public DummyEmailSender(ILogger<DummyEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Development ortamında email göndermek yerine log'a yazıyoruz
            _logger.LogInformation($"Email would be sent to: {email}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {htmlMessage}");
            
            // Gerçek uygulamada burada email gönderme servisi kullanılır
            return Task.CompletedTask;
        }
    }
}
