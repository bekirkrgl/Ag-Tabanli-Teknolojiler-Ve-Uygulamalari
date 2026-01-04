using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HastaneRandevuSistemi.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doktor seçimi gereklidir")]
        [Display(Name = "Doktor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Gün seçimi gereklidir")]
        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Başlangıç saati gereklidir")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Bitiş saati gereklidir")]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
