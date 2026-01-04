using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;
using HastaneRandevuSistemi.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HastaneRandevuSistemi.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DoctorAvailabilityService _availabilityService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public DoctorController(ApplicationDbContext context, DoctorAvailabilityService availabilityService, 
            UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _availabilityService = availabilityService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Doctor
        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive)
                .ToListAsync();
            return View(doctors);
        }

        // GET: Doctor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctor/Panel
        public async Task<IActionResult> Panel()
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Kullanıcının doktor olup olmadığını kontrol et
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                // Kullanıcının rolünü kontrol et
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Doctor"))
                {
                    // Test kullanıcısı için otomatik doktor profili oluştur
                    if (user.Email == "doktor@test.com")
                    {
                        var specialization = await _context.Specializations.FirstOrDefaultAsync();
                        if (specialization != null)
                        {
                            doctor = new Doctor
                            {
                                FirstName = "Test",
                                LastName = "Doktor",
                                Email = user.Email,
                                PhoneNumber = "0555 000 1000",
                                Biography = "Test doktor profili",
                                SpecializationId = specialization.Id,
                                IsActive = true,
                                CreatedDate = DateTime.Now,
                                UserId = user.Id
                            };

                            _context.Doctors.Add(doctor);
                            await _context.SaveChangesAsync();

                            TempData["SuccessMessage"] = "Doktor profiliniz başarıyla oluşturuldu!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Sistem henüz hazır değil. Lütfen daha sonra tekrar deneyin.";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Doktor profiliniz henüz oluşturulmamış. Lütfen sistem yöneticisi ile iletişime geçin.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Bu sayfaya erişim yetkiniz yok. Lütfen doktor hesabınızla giriş yapın.";
                    return RedirectToAction("Index", "Home");
                }
            }

            // TempData mesajlarını kontrol et
            if (TempData["SuccessMessage"] != null)
            {
                Console.WriteLine($"✅ Panel'e dönerken SuccessMessage var: {TempData["SuccessMessage"]}");
            }
            if (TempData["ErrorMessage"] != null)
            {
                Console.WriteLine($"❌ Panel'e dönerken ErrorMessage var: {TempData["ErrorMessage"]}");
            }
            
            // Sadece bu doktorun randevularını getir
            Console.WriteLine("=== DOKTOR PANEL KONTROL ===");
            Console.WriteLine($"Doktor ID: {doctor.Id}");
            Console.WriteLine($"Doktor: {doctor.FirstName} {doctor.LastName}");
            Console.WriteLine($"Doktor Email: {doctor.Email}");
            Console.WriteLine($"Doktor UserId: {doctor.UserId}");
            Console.WriteLine($"Giriş yapan kullanıcı ID: {user.Id}");
            Console.WriteLine($"Giriş yapan kullanıcı Email: {user.Email}");
            
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.Id)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            Console.WriteLine($"Toplam randevu sayısı: {appointments.Count}");
            foreach (var apt in appointments)
            {
                Console.WriteLine($"  - Randevu ID: {apt.Id}, DoctorId: {apt.DoctorId}, Doktor: {apt.Doctor?.FirstName} {apt.Doctor?.LastName}, Hasta: {apt.Patient?.FirstName} {apt.Patient?.LastName}, Tarih: {apt.AppointmentDate}, Saat: {apt.AppointmentTime}");
            }
            Console.WriteLine("============================");

            ViewBag.Doctor = doctor;
            return View(appointments);
        }

        // GET: Doctor/Create
        public IActionResult Create()
        {
            ViewData["SpecializationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Specializations, "Id", "Name");
            return View();
        }

        // POST: Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,Biography,PhotoUrl,SpecializationId,IsActive")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                doctor.CreatedDate = DateTime.Now;
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecializationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Specializations, "Id", "Name", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SpecializationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Specializations, "Id", "Name", doctor.SpecializationId);
            return View(doctor);
        }

        // POST: Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Biography,PhotoUrl,SpecializationId,IsActive,CreatedDate")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
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
            ViewData["SpecializationId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Specializations, "Id", "Name", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                doctor.IsActive = false;
                _context.Update(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }

        // GET: Doctor/Appointments/5
        public async Task<IActionResult> Appointments(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == id)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.Doctor = doctor;
            return View(appointments);
        }

        // GET: Doctor/Availability/5
        public async Task<IActionResult> Availability(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            // Önümüzdeki 30 günün müsait tarihlerini al
            var availableDates = await _availabilityService.GetAvailableDatesAsync(doctor.Id, 30);
            
            ViewBag.Doctor = doctor;
            ViewBag.AvailableDates = availableDates;
            return View();
        }

        // GET: Doctor/GetAvailableDates?doctorId=5
        public async Task<IActionResult> GetAvailableDates(int? doctorId)
        {
            if (doctorId == null)
            {
                return Json(new List<DateTime>());
            }

            var availableDates = await _availabilityService.GetAvailableDatesAsync(doctorId.Value, 30);
            
            return Json(availableDates.Select(d => d.ToString("yyyy-MM-dd")));
        }

        // GET: Doctor/GetAvailableTimeSlots?doctorId=5&date=2025-01-20
        public async Task<IActionResult> GetAvailableTimeSlots(int? doctorId, string date)
        {
            if (doctorId == null || string.IsNullOrEmpty(date))
            {
                return Json(new { success = false, message = "Geçersiz parametreler" });
            }

            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                return Json(new { success = false, message = "Geçersiz tarih formatı" });
            }

            var availableSlots = await _availabilityService.GetAvailableTimeSlotsAsync(doctorId.Value, parsedDate);
            
            return Json(new { 
                success = true, 
                slots = availableSlots.Select(s => new { 
                    date = s.ToString("yyyy-MM-dd"), 
                    time = s.ToString("HH:mm") 
                }) 
            });
        }

        // GET: Doctor/Book/5
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.Doctor = doctor;
            ViewData["PatientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Patients
                    .Where(p => p.IsActive)
                    .Select(p => new { p.Id, Name = p.FullName + " (" + p.Email + ")" })
                    .ToListAsync(), "Id", "Name");

            return View();
        }

        // POST: Doctor/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book([Bind("DoctorId,PatientId,AppointmentDate,AppointmentTime,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Müsaitlik kontrolü
                var isAvailable = await _availabilityService.IsDoctorAvailableAsync(appointment.DoctorId, 
                    appointment.AppointmentDate.Date.Add(appointment.AppointmentTime));
                
                if (!isAvailable)
                {
                    ModelState.AddModelError("", "Seçilen tarih ve saat müsait değil.");
                }
                else
                {
                    appointment.Status = AppointmentStatus.Scheduled;
                    appointment.CreatedDate = DateTime.Now;
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = appointment.DoctorId });
                }
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == appointment.DoctorId);

            ViewBag.Doctor = doctor;
            ViewData["PatientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Patients
                    .Where(p => p.IsActive)
                    .Select(p => new { p.Id, Name = p.FullName + " (" + p.Email + ")" })
                    .ToListAsync(), "Id", "Name", appointment.PatientId);

            return View(appointment);
        }

        // POST: Doctor/ApproveAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }

            // Kullanıcının bu doktorun randevusunu onaylayıp onaylayamayacağını kontrol et
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            
            if (doctor == null || appointment.DoctorId != doctor.Id)
            {
                TempData["ErrorMessage"] = "Bu randevuyu onaylama yetkiniz yok.";
                return RedirectToAction(nameof(Panel));
            }

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.IsConfirmed = true;
            appointment.UpdatedDate = DateTime.Now;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"✅ Randevu onaylandı: {appointment.Patient.FullName} - {appointment.AppointmentDate:dd.MM.yyyy} {appointment.AppointmentTime:hh\\:mm}";
            return RedirectToAction(nameof(Panel));
        }

        // POST: Doctor/RejectAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }

            // Kullanıcının bu doktorun randevusunu reddedip reddedemeyeceğini kontrol et
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            
            if (doctor == null || appointment.DoctorId != doctor.Id)
            {
                TempData["ErrorMessage"] = "Bu randevuyu reddetme yetkiniz yok.";
                return RedirectToAction(nameof(Panel));
            }

            appointment.Status = AppointmentStatus.Cancelled;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu reddedildi.";
            return RedirectToAction(nameof(Panel));
        }

        // POST: Doctor/ConfirmAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAppointment(int? id)
        {
            Console.WriteLine($"=== CONFIRM APPOINTMENT METODU ÇAĞRILDI ===");
            Console.WriteLine($"ID parametresi: {id}");
            
            // ID'yi form'dan veya route'dan al
            if (!id.HasValue || id.Value == 0)
            {
                var idFromForm = Request.Form["id"].FirstOrDefault();
                if (!string.IsNullOrEmpty(idFromForm) && int.TryParse(idFromForm, out int parsedId))
                {
                    id = parsedId;
                    Console.WriteLine($"ID form'dan alındı: {id}");
                }
                else
                {
                    Console.WriteLine($"❌ ID bulunamadı! Form: '{idFromForm}'");
                    TempData["ErrorMessage"] = "Randevu ID'si bulunamadı.";
                    return RedirectToAction(nameof(Panel));
                }
            }
            
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id.Value);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }

            // Kullanıcının bu doktorun randevusunu onaylayıp onaylayamayacağını kontrol et
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            
            if (doctor == null || appointment.DoctorId != doctor.Id)
            {
                Console.WriteLine($"❌ Yetki hatası! Doktor ID: {doctor?.Id}, Randevu DoctorId: {appointment.DoctorId}");
                TempData["ErrorMessage"] = "Bu randevuyu onaylama yetkiniz yok.";
                return RedirectToAction(nameof(Panel));
            }

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.IsConfirmed = true;
            appointment.UpdatedDate = DateTime.Now;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Randevu onaylandı: ID={appointment.Id}, Hasta={appointment.Patient?.FullName}, Tarih={appointment.AppointmentDate:dd.MM.yyyy}, Saat={appointment.AppointmentTime:hh\\:mm}");
            Console.WriteLine($"✅ Veritabanında güncellendi. Status: {appointment.Status}, IsConfirmed: {appointment.IsConfirmed}");
            
            TempData["SuccessMessage"] = $"✅ Randevu onaylandı: {appointment.Patient?.FullName} - {appointment.AppointmentDate:dd.MM.yyyy} {appointment.AppointmentTime:hh\\:mm}";
            Console.WriteLine($"TempData SuccessMessage set edildi: {TempData["SuccessMessage"]}");
            
            return RedirectToAction(nameof(Panel));
        }

        // POST: Doctor/CancelAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int? id)
        {
            Console.WriteLine($"=== CANCEL APPOINTMENT METODU ÇAĞRILDI ===");
            Console.WriteLine($"ID parametresi: {id}");
            
            // ID'yi form'dan veya route'dan al
            if (!id.HasValue || id.Value == 0)
            {
                var idFromForm = Request.Form["id"].FirstOrDefault();
                if (!string.IsNullOrEmpty(idFromForm) && int.TryParse(idFromForm, out int parsedId))
                {
                    id = parsedId;
                    Console.WriteLine($"ID form'dan alındı: {id}");
                }
                else
                {
                    Console.WriteLine($"❌ ID bulunamadı! Form: '{idFromForm}'");
                    TempData["ErrorMessage"] = "Randevu ID'si bulunamadı.";
                    return RedirectToAction(nameof(Panel));
                }
            }
            
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id.Value);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            
            if (doctor == null || appointment.DoctorId != doctor.Id)
            {
                Console.WriteLine($"❌ Yetki hatası! Doktor ID: {doctor?.Id}, Randevu DoctorId: {appointment.DoctorId}");
                TempData["ErrorMessage"] = "Bu randevuyu iptal etme yetkiniz yok.";
                return RedirectToAction(nameof(Panel));
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedDate = DateTime.Now;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            Console.WriteLine($"❌ Randevu iptal edildi: ID={appointment.Id}, Hasta={appointment.Patient?.FullName}, Tarih={appointment.AppointmentDate:dd.MM.yyyy}, Saat={appointment.AppointmentTime:hh\\:mm}");
            Console.WriteLine($"❌ Veritabanında güncellendi. Status: {appointment.Status}");
            
            TempData["SuccessMessage"] = $"❌ Randevu iptal edildi: {appointment.Patient?.FullName} - {appointment.AppointmentDate:dd.MM.yyyy} {appointment.AppointmentTime:hh\\:mm}";
            Console.WriteLine($"TempData SuccessMessage set edildi: {TempData["SuccessMessage"]}");
            
            return RedirectToAction(nameof(Panel));
        }

        // POST: Doctor/CompleteAppointment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }

            // Kullanıcının bu doktorun randevusunu tamamlayıp tamamlayamayacağını kontrol et
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Panel));
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
            
            if (doctor == null || appointment.DoctorId != doctor.Id)
            {
                TempData["ErrorMessage"] = "Bu randevuyu tamamlama yetkiniz yok.";
                return RedirectToAction(nameof(Panel));
            }

            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedDate = DateTime.Now;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu tamamlandı olarak işaretlendi.";
            return RedirectToAction(nameof(Panel));
        }
    }
}
