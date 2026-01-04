using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HastaneRandevuSistemi.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doktor seçimi gereklidir")]
        [Display(Name = "Doktor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Hasta seçimi gereklidir")]
        [Display(Name = "Hasta")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Randevu tarihi gereklidir")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Randevu saati gereklidir")]
        [Display(Name = "Randevu Saati")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(500, ErrorMessage = "Not en fazla 500 karakter olabilir")]
        [Display(Name = "Not")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Randevu durumu gereklidir")]
        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Onaylandı mı?")]
        public bool IsConfirmed { get; set; } = false;

        // Navigation Properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        // Computed property for full appointment datetime
        [NotMapped]
        public DateTime FullAppointmentDateTime => AppointmentDate.Date.Add(AppointmentTime);
    }

    public enum AppointmentStatus
    {
        [Display(Name = "Planlandı")]
        Scheduled = 1,
        [Display(Name = "Onaylandı")]
        Confirmed = 2,
        [Display(Name = "Tamamlandı")]
        Completed = 3,
        [Display(Name = "İptal Edildi")]
        Cancelled = 4,
        [Display(Name = "Gelmedi")]
        NoShow = 5
    }
}
