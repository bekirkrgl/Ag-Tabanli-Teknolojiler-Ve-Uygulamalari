using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HastaneRandevuSistemi.Models
{
    public class AvailabilityBlock
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doktor seçimi gereklidir")]
        [Display(Name = "Doktor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi gereklidir")]
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi gereklidir")]
        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Sebep en fazla 500 karakter olabilir")]
        [Display(Name = "Sebep")]
        public string? Reason { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
