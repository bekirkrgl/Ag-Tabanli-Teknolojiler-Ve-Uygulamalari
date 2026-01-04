using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HastaneRandevuSistemi.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PatientController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Patient
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients
                .Where(p => p.IsActive)
                .ToListAsync();
            return View(patients);
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            // Get patient's appointments
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Where(a => a.PatientId == id)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.Appointments = appointments;
            return View(patient);
        }

        // GET: Patient/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,TcNumber,BirthDate,Address,IsActive")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.CreatedDate = DateTime.Now;
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,TcNumber,BirthDate,Address,IsActive,CreatedDate")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsActive = false;
                _context.Update(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        // GET: Patient/Panel
        public async Task<IActionResult> Panel(string? specialization = null, string? search = null)
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "Giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Kullanıcının hasta olup olmadığını kontrol et
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok. Lütfen hasta hesabınızla giriş yapın.";
                return RedirectToAction("Index", "Home");
            }

            // Hasta istatistikleri - Sadece gerçek veriler
            var totalAppointments = await _context.Appointments
                .CountAsync(a => a.PatientId == patient.Id);

            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Where(a => a.PatientId == patient.Id && a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            var completedAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Where(a => a.PatientId == patient.Id && a.AppointmentDate < DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(8)
                .ToListAsync();

            // Doktorları filtrele
            var doctorsQuery = _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive);

            if (!string.IsNullOrEmpty(specialization))
            {
                doctorsQuery = doctorsQuery.Where(d => d.Specialization.Name == specialization);
            }

            if (!string.IsNullOrEmpty(search))
            {
                doctorsQuery = doctorsQuery.Where(d => 
                    d.FirstName.Contains(search) || 
                    d.LastName.Contains(search) || 
                    d.Specialization.Name.Contains(search));
            }

            var availableDoctors = await doctorsQuery
                .OrderBy(d => d.Specialization.Name)
                .ThenBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();

            // Uzmanlık alanları
            var specializations = await _context.Specializations
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Patient = patient;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.UpcomingAppointments = upcomingAppointments;
            ViewBag.CompletedAppointments = completedAppointments;
            ViewBag.Specializations = specializations;
            ViewBag.SelectedSpecialization = specialization;
            ViewBag.SearchTerm = search;

            return View(availableDoctors);
        }
    }
}
