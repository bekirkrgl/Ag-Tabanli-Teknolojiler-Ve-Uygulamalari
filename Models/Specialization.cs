using System.ComponentModel.DataAnnotations;

namespace HastaneRandevuSistemi.Models
{
    public class Specialization
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branş adı gereklidir")]
        [StringLength(100, ErrorMessage = "Branş adı en fazla 100 karakter olabilir")]
        [Display(Name = "Branş Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
