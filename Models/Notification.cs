using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HastaneRandevuSistemi.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj gereklidir")]
        [StringLength(1000, ErrorMessage = "Mesaj en fazla 1000 karakter olabilir")]
        [Display(Name = "Mesaj")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Tip")]
        public NotificationType Type { get; set; } = NotificationType.Info;

        [Display(Name = "Okundu mu?")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Okunma Tarihi")]
        public DateTime? ReadDate { get; set; }

        // Navigation Properties
        public string? UserId { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? AppointmentId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }

    public enum NotificationType
    {
        [Display(Name = "Bilgi")]
        Info = 1,
        [Display(Name = "Başarı")]
        Success = 2,
        [Display(Name = "Uyarı")]
        Warning = 3,
        [Display(Name = "Hata")]
        Error = 4,
        [Display(Name = "Randevu")]
        Appointment = 5
    }
}
