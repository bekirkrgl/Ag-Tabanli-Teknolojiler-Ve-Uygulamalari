using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HastaneRandevuSistemi.Data;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevuSistemi.Controllers
{
    public class TestController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TestController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> CreateTestUser()
        {
            try
            {
                // Mevcut test kullanıcılarını sil
                var existingUsers = await _userManager.Users.Where(u => u.Email.Contains("@test.com")).ToListAsync();
                foreach (var user in existingUsers)
                {
                    await _userManager.DeleteAsync(user);
                }

                // Test hasta kullanıcısı oluştur
                var testUser = new IdentityUser
                {
                    UserName = "hasta@test.com",
                    Email = "hasta@test.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(testUser, "Hasta123!");
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(testUser, "Patient");
                    
                    // Test hasta kaydı oluştur
                    var testPatient = new Models.Patient
                    {
                        FirstName = "Test",
                        LastName = "Hasta",
                        Email = "hasta@test.com",
                        PhoneNumber = "0555 000 0000",
                        TcNumber = "11111111111",
                        BirthDate = DateTime.Now.AddYears(-30),
                        Address = "Test Adres",
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        UserId = testUser.Id
                    };

                    _context.Patients.Add(testPatient);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = $"Test hasta kullanıcısı başarıyla oluşturuldu! UserId: {testUser.Id}";
                    ViewBag.Email = "hasta@test.com";
                    ViewBag.Password = "Hasta123!";
                    ViewBag.UserId = testUser.Id;
                }
                else
                {
                    ViewBag.Message = "Kullanıcı oluşturulamadı: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Hata: " + ex.Message;
            }

            return View();
        }

        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var patients = await _context.Patients.ToListAsync();
            var doctors = await _context.Doctors.ToListAsync();
            
            // Debug bilgisi
            var testUser = users.FirstOrDefault(u => u.Email == "hasta@test.com");
            var testPatient = patients.FirstOrDefault(p => p.Email == "hasta@test.com");
            var testDoctorUser = users.FirstOrDefault(u => u.Email == "doktor@test.com");
            var testDoctor = doctors.FirstOrDefault(d => d.Email == "doktor@test.com");
            
            // Kullanıcı rollerini kontrol et
            var testUserRoles = testUser != null ? await _userManager.GetRolesAsync(testUser) : new List<string>();
            var testDoctorUserRoles = testDoctorUser != null ? await _userManager.GetRolesAsync(testDoctorUser) : new List<string>();
            
            ViewBag.Users = users;
            ViewBag.Patients = patients;
            ViewBag.Doctors = doctors;
            ViewBag.TestUser = testUser;
            ViewBag.TestPatient = testPatient;
            ViewBag.TestDoctorUser = testDoctorUser;
            ViewBag.TestDoctor = testDoctor;
            ViewBag.UserPatientMatch = testUser != null && testPatient != null && testUser.Id == testPatient.UserId;
            ViewBag.UserDoctorMatch = testDoctorUser != null && testDoctor != null && testDoctorUser.Id == testDoctor.UserId;
            ViewBag.TestUserRoles = testUserRoles;
            ViewBag.TestDoctorUserRoles = testDoctorUserRoles;
            
            return View();
        }

        public async Task<IActionResult> QuickTest()
        {
            try
            {
                // Tüm kullanıcıları sil
                var allUsers = await _userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    await _userManager.DeleteAsync(user);
                }

                // Tüm hastaları sil
                _context.Patients.RemoveRange(_context.Patients);
                await _context.SaveChangesAsync();

                // Basit test kullanıcısı oluştur
                var patientUser = new IdentityUser
                {
                    UserName = "hasta@test.com",
                    Email = "hasta@test.com",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(patientUser, "Hasta123!");
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(patientUser, "Patient");
                    
                    var patient = new Models.Patient
                    {
                        FirstName = "Test",
                        LastName = "Hasta",
                        Email = "hasta@test.com",
                        PhoneNumber = "0555 000 0000",
                        TcNumber = "11111111111",
                        BirthDate = DateTime.Now.AddYears(-30),
                        Address = "Test Adres",
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        UserId = patientUser.Id
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    // Doktor kullanıcısı da oluştur
                    var doctorUser = new IdentityUser
                    {
                        UserName = "doktor@test.com",
                        Email = "doktor@test.com",
                        EmailConfirmed = true
                    };

                    var doctorResult = await _userManager.CreateAsync(doctorUser, "Doktor123!");
                    if (doctorResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(doctorUser, "Doctor");
                        
                        var doctor = new Models.Doctor
                        {
                            FirstName = "Test",
                            LastName = "Doktor",
                            Email = "doktor@test.com",
                            PhoneNumber = "0555 000 1000",
                            Biography = "Test doktor",
                            SpecializationId = 1, // Kardiyoloji
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            UserId = doctorUser.Id
                        };

                        _context.Doctors.Add(doctor);
                        await _context.SaveChangesAsync();
                    }

                    ViewBag.Message = "✅ BAŞARILI! Test kullanıcıları oluşturuldu!";
                    ViewBag.Email = "hasta@test.com";
                    ViewBag.Password = "Hasta123!";
                    ViewBag.DoctorEmail = "doktor@test.com";
                    ViewBag.DoctorPassword = "Doktor123!";
                    ViewBag.UserId = patientUser.Id;
                    ViewBag.PatientId = patient.Id;
                }
                else
                {
                    ViewBag.Message = "❌ HATA: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "❌ HATA: " + ex.Message;
            }

            return View("CreateTestUser");
        }

        public async Task<IActionResult> ResetUsers()
        {
            try
            {
                // Tüm kullanıcıları sil
                var allUsers = await _userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    await _userManager.DeleteAsync(user);
                }

                // Tüm hastaları ve doktorları sil
                _context.Patients.RemoveRange(_context.Patients);
                _context.Doctors.RemoveRange(_context.Doctors);
                await _context.SaveChangesAsync();

                // Hasta kullanıcısı oluştur
                var patientUser = new IdentityUser
                {
                    UserName = "hasta@test.com",
                    Email = "hasta@test.com",
                    EmailConfirmed = true
                };

                var patientResult = await _userManager.CreateAsync(patientUser, "Hasta123!");
                
                if (patientResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(patientUser, "Patient");
                    
                    var patient = new Models.Patient
                    {
                        FirstName = "Test",
                        LastName = "Hasta",
                        Email = "hasta@test.com",
                        PhoneNumber = "0555 000 0000",
                        TcNumber = "11111111111",
                        BirthDate = DateTime.Now.AddYears(-30),
                        Address = "Test Adres",
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        UserId = patientUser.Id
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();
                }

                // Doktor kullanıcısı oluştur
                var doctorUser = new IdentityUser
                {
                    UserName = "doktor@test.com",
                    Email = "doktor@test.com",
                    EmailConfirmed = true
                };

                var doctorResult = await _userManager.CreateAsync(doctorUser, "Doktor123!");
                if (doctorResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(doctorUser, "Doctor");
                    
                    // İlk branşı al
                    var specialization = await _context.Specializations.FirstOrDefaultAsync();
                    if (specialization != null)
                    {
                        var doctor = new Models.Doctor
                        {
                            FirstName = "Test",
                            LastName = "Doktor",
                            Email = "doktor@test.com",
                            PhoneNumber = "0555 000 1000",
                            Biography = "Test doktor",
                            SpecializationId = specialization.Id,
                            IsActive = true,
                            CreatedDate = DateTime.Now,
                            UserId = doctorUser.Id
                        };

                        _context.Doctors.Add(doctor);
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "Test kullanıcıları başarıyla yeniden oluşturuldu!";
                return RedirectToAction("CreateTestUser");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
                return RedirectToAction("CreateTestUser");
            }
        }
    }
}
