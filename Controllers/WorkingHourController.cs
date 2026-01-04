using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;
using Microsoft.AspNetCore.Identity;

namespace HastaneRandevuSistemi.Controllers
{
    public class WorkingHourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkingHourController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WorkingHour
        public async Task<IActionResult> Index()
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doktor profiliniz bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var workingHours = await _context.WorkingHours
                .Include(w => w.Doctor)
                .ThenInclude(d => d.Specialization)
                .Where(w => w.DoctorId == doctor.Id && w.IsActive)
                .OrderBy(w => w.DayOfWeek)
                .ThenBy(w => w.StartTime)
                .ToListAsync();

            ViewBag.Doctor = doctor;
            return View(workingHours);
        }

        // GET: WorkingHour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHour = await _context.WorkingHours
                .Include(w => w.Doctor)
                .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workingHour == null)
            {
                return NotFound();
            }

            return View(workingHour);
        }

        // GET: WorkingHour/Create
        public async Task<IActionResult> Create()
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doktor profiliniz bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Doctor = doctor;
            return View();
        }

        // POST: WorkingHour/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DayOfWeek,StartTime,EndTime,IsActive")] WorkingHour workingHour)
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var user = await _userManager.GetUserAsync(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Doktor profiliniz bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                workingHour.DoctorId = doctor.Id;
                _context.Add(workingHour);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Çalışma saati başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Doctor = doctor;
            return View(workingHour);
        }

        // GET: WorkingHour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHour = await _context.WorkingHours.FindAsync(id);
            if (workingHour == null)
            {
                return NotFound();
            }

            ViewData["DoctorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Doctors
                    .Include(d => d.Specialization)
                    .Where(d => d.IsActive)
                    .Select(d => new { d.Id, Name = d.FullName + " - " + d.Specialization.Name })
                    .ToListAsync(), "Id", "Name", workingHour.DoctorId);

            return View(workingHour);
        }

        // POST: WorkingHour/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,DayOfWeek,StartTime,EndTime,IsActive")] WorkingHour workingHour)
        {
            if (id != workingHour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workingHour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkingHourExists(workingHour.Id))
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

            ViewData["DoctorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Doctors
                    .Include(d => d.Specialization)
                    .Where(d => d.IsActive)
                    .Select(d => new { d.Id, Name = d.FullName + " - " + d.Specialization.Name })
                    .ToListAsync(), "Id", "Name", workingHour.DoctorId);

            return View(workingHour);
        }

        // GET: WorkingHour/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHour = await _context.WorkingHours
                .Include(w => w.Doctor)
                .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workingHour == null)
            {
                return NotFound();
            }

            return View(workingHour);
        }

        // POST: WorkingHour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workingHour = await _context.WorkingHours.FindAsync(id);
            if (workingHour != null)
            {
                workingHour.IsActive = false;
                _context.Update(workingHour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkingHourExists(int id)
        {
            return _context.WorkingHours.Any(e => e.Id == id);
        }
    }
}
