using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Models;

namespace HastaneRandevuSistemi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<AvailabilityBlock> AvailabilityBlocks { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Doctor - Specialization relationship
            builder.Entity<Doctor>()
                .HasOne(d => d.Specialization)
                .WithMany(s => s.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - Appointments relationship
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient - Appointments relationship
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - WorkingHours relationship
            builder.Entity<WorkingHour>()
                .HasOne(w => w.Doctor)
                .WithMany(d => d.WorkingHours)
                .HasForeignKey(w => w.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor - AvailabilityBlocks relationship
            builder.Entity<AvailabilityBlock>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.AvailabilityBlocks)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            builder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            builder.Entity<Patient>()
                .HasIndex(p => p.Email)
                .IsUnique();

            builder.Entity<Patient>()
                .HasIndex(p => p.TcNumber)
                .IsUnique();

            // Seed data for Specializations
            builder.Entity<Specialization>().HasData(
                new Specialization { Id = 1, Name = "Kardiyoloji", Description = "Kalp ve damar hastalıkları", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 2, Name = "Nöroloji", Description = "Sinir sistemi hastalıkları", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 3, Name = "Ortopedi", Description = "Kemik ve eklem hastalıkları", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 4, Name = "Dermatoloji", Description = "Cilt hastalıkları", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 5, Name = "Göz Hastalıkları", Description = "Göz ve görme problemleri", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 6, Name = "Kulak Burun Boğaz", Description = "KBB hastalıkları", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 7, Name = "İç Hastalıkları", Description = "Genel iç hastalıkları", CreatedDate = new DateTime(2025, 1, 1) },
                new Specialization { Id = 8, Name = "Çocuk Sağlığı", Description = "Çocuk hastalıkları", CreatedDate = new DateTime(2025, 1, 1) }
            );
        }
    }
}
