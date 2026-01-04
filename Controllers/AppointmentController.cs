using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HastaneRandevuSistemi.Data;
using HastaneRandevuSistemi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HastaneRandevuSistemi.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointment
        public async Task<IActionResult> Index()
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/Identity/Account/Login");
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

            // Sadece bu hastanın randevularını getir
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patient.Id)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
            
            return View(appointments);
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointment/Create
        public async Task<IActionResult> Create(int? doctorId)
        {
            try
            {
                // Giriş yapmış kullanıcıyı kontrol et
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    var returnUrl = doctorId.HasValue 
                        ? $"/Appointment/Create?doctorId={doctorId.Value}" 
                        : "/Appointment/Create";
                    return Redirect($"/Identity/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
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

                // Doktorları getir
                var doctors = await _context.Doctors
                    .Include(d => d.Specialization)
                    .Where(d => d.IsActive)
                    .ToListAsync();

                // Doktor listesini oluştur
                var doctorSelectItems = doctors.Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                { 
                    Value = d.Id.ToString(), 
                    Text = d.FirstName + " " + d.LastName + " - " + (d.Specialization != null ? d.Specialization.Name : "Branş Belirtilmemiş") 
                }).ToList();

                ViewBag.DoctorId = doctorSelectItems;

                // Hasta bilgilerini ViewBag'e ekle
                ViewBag.Patient = patient;
                ViewBag.PatientId = patient.Id; // Otomatik seçim için

                // Eğer doctorId parametresi varsa, o doktoru seçili yap
                if (doctorId.HasValue && doctorId.Value > 0)
                {
                    var selectedDoctor = doctors.FirstOrDefault(d => d.Id == doctorId.Value);
                    if (selectedDoctor != null)
                    {
                        ViewBag.SelectedDoctorId = selectedDoctor.Id;
                        ViewBag.SelectedDoctor = selectedDoctor;
                        Console.WriteLine($"Selected Doctor: {selectedDoctor.FirstName} {selectedDoctor.LastName} - {selectedDoctor.Specialization?.Name}");
                    }
                }

                // Debug bilgisi
                ViewBag.DoctorCount = doctors.Count;
                ViewBag.PatientCount = 1; // Sadece giriş yapmış hasta
                
                // Log for debugging
                Console.WriteLine($"Create GET: {doctors.Count} doctors found");
                foreach (var doctor in doctors)
                {
                    Console.WriteLine($"Doctor: {doctor.FirstName} {doctor.LastName} - {doctor.Specialization?.Name}");
                }
                Console.WriteLine($"SelectList count: {doctorSelectItems.Count}");
                foreach (var item in doctorSelectItems)
                {
                    Console.WriteLine($"SelectListItem: {item.Text} (Value: {item.Value})");
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda boş listeler
                Console.WriteLine($"Error in Create GET action: {ex.Message}");
                ViewBag.DoctorId = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,Notes")] Appointment appointment)
        {
            Console.WriteLine(">>> CREATE POST BAŞLADI <<<");
            
            try
            {
                Console.WriteLine("Create POST - try bloğuna girdi");
                
                // Tüm form değerlerini logla
                Console.WriteLine("=== FORM VERİLERİ ===");
                foreach (var key in Request.Form.Keys)
                {
                    var value = Request.Form[key].ToString();
                    Console.WriteLine($"Form[{key}] = '{value}' (Length: {value.Length})");
                }
                Console.WriteLine("=== FORM VERİLERİ SON ===");
                
                // Form'dan gelen tarih ve saat bilgilerini manuel olarak parse et
                // asp-for ile oluşturulan input'lar genelde model property adıyla aynı name'e sahiptir
                var appointmentDateStr = Request.Form["AppointmentDate"].FirstOrDefault() ?? 
                                         Request.Form["appointment.AppointmentDate"].FirstOrDefault() ?? 
                                         string.Empty;
                var appointmentTimeStr = Request.Form["AppointmentTime"].FirstOrDefault() ?? 
                                        Request.Form["appointment.AppointmentTime"].FirstOrDefault() ?? 
                                        string.Empty;
                
                // Alternatif: doğrudan input name'lerini dene
                if (string.IsNullOrEmpty(appointmentDateStr))
                {
                    appointmentDateStr = Request.Form["appointmentDate"].FirstOrDefault() ?? string.Empty;
                }
                if (string.IsNullOrEmpty(appointmentTimeStr))
                {
                    appointmentTimeStr = Request.Form["appointmentTime"].FirstOrDefault() ?? string.Empty;
                }
                
                Console.WriteLine($"Raw AppointmentDate: '{appointmentDateStr}' (Length: {appointmentDateStr?.Length ?? 0})");
                Console.WriteLine($"Raw AppointmentTime: '{appointmentTimeStr}' (Length: {appointmentTimeStr?.Length ?? 0})");
                
                // Giriş yapmış kullanıcıyı kontrol et
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return Redirect("/Identity/Account/Login");
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

                // Hasta ID'sini otomatik ata
                appointment.PatientId = patient.Id;
                
                // Tarih ve saat parsing için isValid değişkeni
                bool isValid = true;
                
                // DoctorId'yi kontrol et ve form'dan al
                Console.WriteLine($"DEBUG: appointment.DoctorId = {appointment.DoctorId}");
                if (appointment.DoctorId == 0)
                {
                    // Model binding çalışmamış, form'dan manuel al
                    var doctorIdStr = Request.Form["DoctorId"].FirstOrDefault() ?? 
                                     Request.Form["doctorSelect"].FirstOrDefault() ?? 
                                     string.Empty;
                    Console.WriteLine($"DEBUG: DoctorId form'dan: '{doctorIdStr}'");
                    if (!string.IsNullOrEmpty(doctorIdStr) && int.TryParse(doctorIdStr, out int doctorId) && doctorId > 0)
                    {
                        appointment.DoctorId = doctorId;
                        Console.WriteLine($"✅ DoctorId form'dan alındı: {appointment.DoctorId}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ DoctorId parse edilemedi veya 0: '{doctorIdStr}'");
                        TempData["ErrorMessage"] = "⚠️ Lütfen bir doktor seçin.";
                        isValid = false;
                    }
                }
                else
                {
                    Console.WriteLine($"✅ DoctorId model binding'den geldi: {appointment.DoctorId}");
                }
                
                // Status'u otomatik ata
                appointment.Status = AppointmentStatus.Scheduled;
                appointment.IsConfirmed = false; // Başlangıçta false
                
                // CreatedDate'i otomatik ata
                appointment.CreatedDate = DateTime.Now;
                
                // Tarih ve saat parsing değişkenleri
                DateTime parsedDate = default;
                TimeSpan parsedTime = default;
                
                // Tarih parse et
                if (!string.IsNullOrEmpty(appointmentDateStr) && DateTime.TryParse(appointmentDateStr, out parsedDate))
                {
                    appointment.AppointmentDate = parsedDate.Date;
                    Console.WriteLine($"✅ Tarih parse edildi: {appointment.AppointmentDate}");
                }
                else
                {
                    Console.WriteLine($"❌ Tarih parse edilemedi: '{appointmentDateStr}'");
                    TempData["ErrorMessage"] = "⚠️ Lütfen geçerli bir randevu tarihi seçin.";
                    isValid = false;
                }
                
                // Saat parse et
                if (isValid && !string.IsNullOrEmpty(appointmentTimeStr))
                {
                    if (TimeSpan.TryParse(appointmentTimeStr, out parsedTime))
                    {
                        appointment.AppointmentTime = parsedTime;
                        Console.WriteLine($"✅ Saat parse edildi: {appointment.AppointmentTime}");
                    }
                    else
                    {
                        // HH:mm formatındaysa parse et
                        var timeParts = appointmentTimeStr.Split(':');
                        if (timeParts.Length == 2 && 
                            int.TryParse(timeParts[0], out int hour) && 
                            int.TryParse(timeParts[1], out int minute))
                        {
                            appointment.AppointmentTime = new TimeSpan(hour, minute, 0);
                            parsedTime = appointment.AppointmentTime;
                            Console.WriteLine($"✅ Saat parse edildi (manuel): {appointment.AppointmentTime}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Saat parse edilemedi: '{appointmentTimeStr}'");
                            TempData["ErrorMessage"] = "⚠️ Lütfen geçerli bir randevu saati seçin.";
                            isValid = false;
                        }
                    }
                }
                else if (isValid && string.IsNullOrEmpty(appointmentTimeStr))
                {
                    Console.WriteLine($"❌ Saat boş!");
                    TempData["ErrorMessage"] = "⚠️ Lütfen geçerli bir randevu saati seçin.";
                    isValid = false;
                }

                Console.WriteLine("=== RANDEVU OLUŞTURMA DEBUG ===");
                Console.WriteLine($"DoctorId: {appointment.DoctorId}");
                Console.WriteLine($"PatientId: {appointment.PatientId}");
                Console.WriteLine($"AppointmentDate: {appointment.AppointmentDate}");
                Console.WriteLine($"AppointmentTime: {appointment.AppointmentTime}");
                Console.WriteLine($"Status: {appointment.Status}");
                Console.WriteLine($"Notes: {appointment.Notes}");
                
                // Temel validasyon
                if (appointment.DoctorId <= 0)
                {
                    TempData["ErrorMessage"] = "⚠️ Lütfen bir doktor seçin.";
                    isValid = false;
                }
                else if (appointment.AppointmentDate == default(DateTime))
                {
                    TempData["ErrorMessage"] = "⚠️ Lütfen geçerli bir randevu tarihi seçin.";
                    isValid = false;
                }
                else if (appointment.AppointmentTime == default(TimeSpan))
                {
                    TempData["ErrorMessage"] = "⚠️ Lütfen geçerli bir randevu saati seçin.";
                    isValid = false;
                }
                else
                {
                    isValid = true;
                    
                    // Geçmiş tarih kontrolü
                    var today = DateTime.Today;
                    if (appointment.AppointmentDate.Date < today)
                    {
                        Console.WriteLine($"Geçmiş tarih seçildi: {appointment.AppointmentDate.Date}");
                        TempData["ErrorMessage"] = "⚠️ Geçmiş bir tarihe randevu alamazsınız. Lütfen bugünden itibaren bir tarih seçin.";
                        isValid = false;
                    }
                    
                    // Hafta sonu kontrolü (Cumartesi=6, Pazar=0)
                    if (isValid)
                    {
                        var dayOfWeek = appointment.AppointmentDate.DayOfWeek;
                        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                        {
                            TempData["ErrorMessage"] = "⚠️ Hafta sonu randevu alamazsınız. Lütfen hafta içi (Pazartesi-Cuma) bir tarih seçin.";
                            isValid = false;
                        }
                    }
                    
                    // Mesai saatleri kontrolü (08:00 - 17:00 arası, 17:00 dahil)
                    if (isValid)
                    {
                        var appointmentHour = appointment.AppointmentTime.Hours;
                        var appointmentMinute = appointment.AppointmentTime.Minutes;
                        // 08:00 - 17:00 arası olmalı (17:00 dahil, 17:01 geçersiz)
                        if (appointmentHour < 8 || appointmentHour > 17 || (appointmentHour == 17 && appointmentMinute > 0))
                        {
                            Console.WriteLine($"Geçersiz saat seçildi: {appointment.AppointmentTime}");
                            TempData["ErrorMessage"] = "⚠️ Randevu saatleri sadece mesai saatleri içindedir (08:00 - 17:00). Lütfen geçerli bir saat seçin.";
                            isValid = false;
                        }
                    }
                
                if (isValid)
                {
                    Console.WriteLine("Validasyon geçti, randevu ekleniyor...");
                    
                    // Doktor bilgisini al ve logla
                    var doctorForAppointment = await _context.Doctors
                        .Include(d => d.Specialization)
                        .FirstOrDefaultAsync(d => d.Id == appointment.DoctorId);
                    
                    Console.WriteLine("=== RANDEVU KAYDETME ÖNCESİ KONTROL ===");
                    Console.WriteLine($"DoctorId: {appointment.DoctorId}");
                    Console.WriteLine($"Doktor: {doctorForAppointment?.FirstName} {doctorForAppointment?.LastName}");
                    Console.WriteLine($"Doktor Email: {doctorForAppointment?.Email}");
                    Console.WriteLine($"PatientId: {appointment.PatientId}");
                    Console.WriteLine($"AppointmentDate: {appointment.AppointmentDate}");
                    Console.WriteLine($"AppointmentTime: {appointment.AppointmentTime}");
                    Console.WriteLine($"Status: {appointment.Status}");
                    Console.WriteLine($"IsConfirmed: {appointment.IsConfirmed}");
                    Console.WriteLine("=========================================");
                    
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"✅ Randevu başarıyla eklendi! ID: {appointment.Id}");
                    
                    // Randevuyu veritabanından tekrar oku ve doğrula
                    var savedAppointment = await _context.Appointments
                        .Include(a => a.Doctor)
                        .Include(a => a.Patient)
                        .FirstOrDefaultAsync(a => a.Id == appointment.Id);
                    
                    Console.WriteLine("=== RANDEVU KAYDETME SONRASI DOĞRULAMA ===");
                    Console.WriteLine($"Randevu ID: {savedAppointment?.Id}");
                    Console.WriteLine($"DoctorId: {savedAppointment?.DoctorId}");
                    Console.WriteLine($"Doktor: {savedAppointment?.Doctor?.FirstName} {savedAppointment?.Doctor?.LastName}");
                    Console.WriteLine($"Doktor Email: {savedAppointment?.Doctor?.Email}");
                    Console.WriteLine($"PatientId: {savedAppointment?.PatientId}");
                    Console.WriteLine($"Hasta: {savedAppointment?.Patient?.FirstName} {savedAppointment?.Patient?.LastName}");
                    Console.WriteLine($"AppointmentDate: {savedAppointment?.AppointmentDate}");
                    Console.WriteLine($"AppointmentTime: {savedAppointment?.AppointmentTime}");
                    Console.WriteLine("============================================");
                    
                    TempData["SuccessMessage"] = "✅ Randevunuz başarıyla oluşturuldu!";
                    return RedirectToAction("Index", "Appointment");
                }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create POST: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Randevu oluşturulurken hata oluştu: " + ex.Message;
            }

            // View'a dönmeden önce gerekli verileri hazırla
            var doctorsForList = await _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive)
                .ToListAsync();
            
            var doctorSelectItems = doctorsForList.Select(d => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            { 
                Value = d.Id.ToString(), 
                Text = d.FirstName + " " + d.LastName + " - " + (d.Specialization != null ? d.Specialization.Name : "Branş Belirtilmemiş")
            }).ToList();

            ViewBag.DoctorId = doctorSelectItems;
            
            // Hasta bilgilerini tekrar yükle
            var userForView = await _userManager.GetUserAsync(User);
            if (userForView != null)
            {
                var patientForView = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == userForView.Id);
                if (patientForView != null)
                {
                    ViewBag.Patient = patientForView;
                    ViewBag.PatientId = patientForView.Id;
                }
            }

            ViewBag.DoctorCount = doctorsForList.Count;
            ViewBag.PatientCount = 1;

            return View(appointment);
        }

        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            ViewData["DoctorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Doctors
                    .Include(d => d.Specialization)
                    .Where(d => d.IsActive)
                    .Select(d => new { d.Id, Name = d.FullName + " - " + d.Specialization.Name })
                    .ToListAsync(), "Id", "Name", appointment.DoctorId);

            ViewData["PatientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Patients
                    .Where(p => p.IsActive)
                    .Select(p => new { p.Id, Name = p.FullName + " (" + p.Email + ")" })
                    .ToListAsync(), "Id", "Name", appointment.PatientId);

            return View(appointment);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DoctorId,PatientId,AppointmentDate,AppointmentTime,Notes,Status,CreatedDate")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.UpdatedDate = DateTime.Now;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
                    .ToListAsync(), "Id", "Name", appointment.DoctorId);

            ViewData["PatientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Patients
                    .Where(p => p.IsActive)
                    .Select(p => new { p.Id, Name = p.FullName + " (" + p.Email + ")" })
                    .ToListAsync(), "Id", "Name", appointment.PatientId);

            return View(appointment);
        }

        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Specialization)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        // POST: Appointment/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/Identity/Account/Login");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            // Doktor yetkisi kontrolü
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null || doctor.Id != appointment.DoctorId)
            {
                TempData["ErrorMessage"] = "Bu randevuyu onaylama yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.IsConfirmed = true;
            appointment.UpdatedDate = DateTime.Now;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla onaylandı!";
            return RedirectToAction("Panel", "Doctor");
        }

        // POST: Appointment/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/Identity/Account/Login");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            // Doktor yetkisi kontrolü
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null || doctor.Id != appointment.DoctorId)
            {
                TempData["ErrorMessage"] = "Bu randevuyu reddetme yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.IsConfirmed = false;
            appointment.UpdatedDate = DateTime.Now;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu reddedildi!";
            return RedirectToAction("Panel", "Doctor");
        }

        // POST: Appointment/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            // Giriş yapmış kullanıcıyı kontrol et
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/Identity/Account/Login");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            // Doktor yetkisi kontrolü
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null || doctor.Id != appointment.DoctorId)
            {
                TempData["ErrorMessage"] = "Bu randevuyu tamamlama yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedDate = DateTime.Now;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu tamamlandı olarak işaretlendi!";
            return RedirectToAction("Panel", "Doctor");
        }

        // GET: Appointment/Book/5
        public async Task<IActionResult> Book(int? doctorId)
        {
            // Tüm doktorları al
            var doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive)
                .ToListAsync();

            var doctorList = doctors.Select(d => new 
            { 
                Id = d.Id, 
                Name = d.FirstName + " " + d.LastName + " - " + (d.Specialization != null ? d.Specialization.Name : "Branş Belirtilmemiş") 
            }).ToList();

            ViewBag.DoctorId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(doctorList, "Id", "Name", doctorId);

            // Eğer specific doctor seçildiyse
            if (doctorId != null)
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Specialization)
                    .Include(d => d.WorkingHours)
                    .Include(d => d.AvailabilityBlocks)
                    .FirstOrDefaultAsync(m => m.Id == doctorId);

                if (doctor == null)
                {
                    return NotFound();
                }

                ViewBag.Doctor = doctor;
            }

            // Giriş yapmış kullanıcıyı kontrol et ve hasta bilgilerini al
            var currentUser = await _userManager.GetUserAsync(User);
            Patient? currentPatient = null;
            
            if (currentUser != null)
            {
                currentPatient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);
                
                // Giriş yapmış kullanıcı hastaysa onu seçili göster
                if (currentPatient != null)
                {
                    ViewBag.CurrentPatient = currentPatient;
                    ViewBag.PatientId = currentPatient.Id;
                }
            }
            
            ViewData["PatientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Patients
                    .Where(p => p.IsActive)
                    .Select(p => new { p.Id, Name = p.FullName + " (" + p.Email + ")" })
                    .ToListAsync(), "Id", "Name", currentPatient?.Id);

            return View();
        }

        // POST: Appointment/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book([Bind("DoctorId,PatientId,AppointmentDate,AppointmentTime,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Status = AppointmentStatus.Scheduled;
                appointment.CreatedDate = DateTime.Now;
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = appointment.Id });
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.WorkingHours)
                .Include(d => d.AvailabilityBlocks)
                .FirstOrDefaultAsync(m => m.Id == appointment.DoctorId);

            ViewBag.Doctor = doctor;
            
            // Doktorları tekrar yükle
            var doctorsForList = await _context.Doctors
                .Include(d => d.Specialization)
                .Where(d => d.IsActive)
                .ToListAsync();

            var doctorListForSelect = doctorsForList.Select(d => new 
            { 
                Id = d.Id, 
                Name = d.FirstName + " " + d.LastName + " - " + (d.Specialization != null ? d.Specialization.Name : "Branş Belirtilmemiş") 
            }).ToList();

            ViewBag.DoctorId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(doctorListForSelect, "Id", "Name", appointment.DoctorId);
            
            ViewData["PatientId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Patients
                    .Where(p => p.IsActive)
                    .Select(p => new { p.Id, Name = p.FullName + " (" + p.Email + ")" })
                    .ToListAsync(), "Id", "Name", appointment.PatientId);

            return View(appointment);
        }
    }
}
