using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;
using Microsoft.AspNetCore.Identity;

namespace HastaneRandevuSistemi.Controllers
{
    public class AvailabilityBlockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AvailabilityBlockController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AvailabilityBlock
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

            var availabilityBlocks = await _context.AvailabilityBlocks
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Where(a => a.DoctorId == doctor.Id && a.IsActive)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();

            ViewBag.Doctor = doctor;
            return View(availabilityBlocks);
        }

        // GET: AvailabilityBlock/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availabilityBlock = await _context.AvailabilityBlocks
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (availabilityBlock == null)
            {
                return NotFound();
            }

            return View(availabilityBlock);
        }

        // GET: AvailabilityBlock/Create
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

        // POST: AvailabilityBlock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDateTime,EndDateTime,Description,IsActive")] AvailabilityBlock availabilityBlock)
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
                availabilityBlock.DoctorId = doctor.Id;
                availabilityBlock.CreatedDate = DateTime.Now;
                _context.Add(availabilityBlock);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Müsaitlik başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Doctor = doctor;
            return View(availabilityBlock);
        }

        // GET: AvailabilityBlock/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availabilityBlock = await _context.AvailabilityBlocks.FindAsync(id);
            if (availabilityBlock == null)
            {
                return NotFound();
            }

            ViewData["DoctorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Doctors
                    .Include(d => d.Specialization)
                    .Where(d => d.IsActive)
                    .Select(d => new { d.Id, Name = d.FullName + " - " + d.Specialization.Name })
                    .ToListAsync(), "Id", "Name", availabilityBlock.DoctorId);

            return View(availabilityBlock);
        }

        // POST: AvailabilityBlock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,StartDateTime,EndDateTime,Description,IsActive,CreatedDate")] AvailabilityBlock availabilityBlock)
        {
            if (id != availabilityBlock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availabilityBlock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailabilityBlockExists(availabilityBlock.Id))
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
                    .ToListAsync(), "Id", "Name", availabilityBlock.DoctorId);

            return View(availabilityBlock);
        }

        // GET: AvailabilityBlock/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availabilityBlock = await _context.AvailabilityBlocks
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (availabilityBlock == null)
            {
                return NotFound();
            }

            return View(availabilityBlock);
        }

        // POST: AvailabilityBlock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availabilityBlock = await _context.AvailabilityBlocks.FindAsync(id);
            if (availabilityBlock != null)
            {
                availabilityBlock.IsActive = false;
                _context.Update(availabilityBlock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvailabilityBlockExists(int id)
        {
            return _context.AvailabilityBlocks.Any(e => e.Id == id);
        }
    }
}
