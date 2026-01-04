using HastaneRandevuSistemi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HastaneRandevuSistemi.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            // Veritabanını oluştur
            context.Database.EnsureCreated();

            // Sadece test kullanıcıları varsa temizle, yoksa oluştur
            var existingTestPatient = await userManager.FindByEmailAsync("hasta@test.com");
            var existingTestDoctor = await userManager.FindByEmailAsync("doktor@test.com");
            
            // Test kullanıcıları yoksa oluştur
            if (existingTestPatient == null && existingTestDoctor == null)
            {
                // Test kullanıcıları oluştur
                var testPatientUser = new IdentityUser
                {
                    UserName = "hasta@test.com",
                    Email = "hasta@test.com",
                    EmailConfirmed = true
                };

                var testDoctorUser = new IdentityUser
                {
                    UserName = "doktor@test.com",
                    Email = "doktor@test.com",
                    EmailConfirmed = true
                };

                var patientResult = await userManager.CreateAsync(testPatientUser, "Hasta123!");
                var doctorResult = await userManager.CreateAsync(testDoctorUser, "Doktor123!");

                if (patientResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(testPatientUser, "Patient");
                    Console.WriteLine($"Hasta kullanıcısı oluşturuldu: {testPatientUser.Id}");
                }
                else
                {
                    Console.WriteLine($"Hasta kullanıcısı oluşturulamadı: {string.Join(", ", patientResult.Errors.Select(e => e.Description))}");
                }

                if (doctorResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(testDoctorUser, "Doctor");
                    Console.WriteLine($"Doktor kullanıcısı oluşturuldu: {testDoctorUser.Id}");
                }
                else
                {
                    Console.WriteLine($"Doktor kullanıcısı oluşturulamadı: {string.Join(", ", doctorResult.Errors.Select(e => e.Description))}");
                }
            }

            // Örnek Branşlar Ekle (sadece yoksa)
            if (!context.Specializations.Any())
            {
                var specializations = new Specialization[]
                {
                    new Specialization
                    {
                        Name = "Kardiyoloji",
                        Description = "Kalp ve damar hastalıkları",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Specialization
                    {
                        Name = "Çocuk Sağlığı",
                        Description = "Çocuk hastalıkları ve sağlığı",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Specialization
                    {
                        Name = "Ortopedi",
                        Description = "Kemik ve eklem hastalıkları",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Specialization
                    {
                        Name = "Kadın Hastalıkları",
                        Description = "Kadın sağlığı ve doğum",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Specialization
                    {
                        Name = "Göz Hastalıkları",
                        Description = "Göz sağlığı ve hastalıkları",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Specialization
                    {
                        Name = "Dermatoloji",
                        Description = "Cilt hastalıkları",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    }
                };

                context.Specializations.AddRange(specializations);
                context.SaveChanges();
            }

            // Test Doktorlar Ekle
            if (!context.Doctors.Any()) // Sadece doktor yoksa ekle
            {
                var doctors = new List<Doctor>();
                
                // Test doktor hesabı oluştur
                if (existingTestDoctor == null)
                {
                    var testDoctorForDb = await userManager.FindByEmailAsync("doktor@test.com");
                    if (testDoctorForDb != null)
                    {
                        doctors.Add(new Doctor
                        {
                            FirstName = "Test",
                            LastName = "Doktor",
                            Email = "doktor@test.com",
                            PhoneNumber = "0555 000 1001",
                            Biography = "Kardiyoloji alanında 15 yıllık deneyimli uzman doktor. Kalp hastalıkları, hipertansiyon, aritmi ve koroner arter hastalıkları konularında uzmanlaşmıştır. EKG, ekokardiyografi, holter monitörizasyonu ve stres testi gibi kardiyolojik tetkiklerde deneyimlidir. Hasta odaklı yaklaşımı ve modern tedavi yöntemleri ile tanınır.",
                            PhotoUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=200&h=200&fit=crop&crop=face",
                            SpecializationId = 1,
                            IsActive = true,
                            CreatedDate = new DateTime(2024, 1, 1),
                            UserId = testDoctorForDb.Id
                        });
                    }
                }
                
                // Diğer doktorları ve hesaplarını ekle - Her doktor için kullanıcı hesabı oluştur
                var doctorData = new[]
                {
                    new { Doctor = new Doctor { FirstName = "Dr. Fatma", LastName = "Şahin", Email = "fatma.sahin@hastane.com", PhoneNumber = "0535 456 7890", Biography = "Kadın hastalıkları ve doğum konusunda 12 yıllık deneyim.", PhotoUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=150&h=150&fit=crop&crop=face", SpecializationId = 4, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "fatma.sahin@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Ali", LastName = "Çelik", Email = "ali.celik@hastane.com", PhoneNumber = "0536 567 8901", Biography = "Göz hastalıkları ve lazer cerrahisi konusunda uzman.", PhotoUrl = "https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=150&h=150&fit=crop&crop=face", SpecializationId = 5, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "ali.celik@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Ahmet", LastName = "Yılmaz", Email = "ahmet.yilmaz@hastane.com", PhoneNumber = "0532 123 4567", Biography = "15 yıllık deneyimli kardiyoloji uzmanı. Kalp hastalıkları tedavisinde uzmanlaşmış.", PhotoUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=150&h=150&fit=crop&crop=face", SpecializationId = 1, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "ahmet.yilmaz@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Zeynep", LastName = "Arslan", Email = "zeynep.arslan@hastane.com", PhoneNumber = "0537 678 9012", Biography = "Çocuk sağlığı ve hastalıkları konusunda uzman. 8 yıllık deneyim.", PhotoUrl = "https://images.unsplash.com/photo-1594824388852-8b04b7311fef?w=150&h=150&fit=crop&crop=face", SpecializationId = 2, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "zeynep.arslan@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Ayşe", LastName = "Demir", Email = "ayse.demir@hastane.com", PhoneNumber = "0533 234 5678", Biography = "Ortopedi alanında 10 yıllık deneyim. Kemik ve eklem hastalıkları konusunda uzman.", PhotoUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=150&h=150&fit=crop&crop=face", SpecializationId = 3, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "ayse.demir@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Mehmet", LastName = "Kaya", Email = "mehmet.kaya@hastane.com", PhoneNumber = "0534 345 6789", Biography = "Dermatoloji uzmanı. Cilt hastalıkları ve estetik dermatoloji konusunda 12 yıllık deneyim.", PhotoUrl = "https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=150&h=150&fit=crop&crop=face", SpecializationId = 6, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "mehmet.kaya@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Elif", LastName = "Özkan", Email = "elif.ozkan@hastane.com", PhoneNumber = "0538 456 7890", Biography = "Kadın hastalıkları ve doğum uzmanı. Jinekoloji onkolojisi konusunda uzmanlaşmış.", PhotoUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=150&h=150&fit=crop&crop=face", SpecializationId = 4, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "elif.ozkan@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Burak", LastName = "Şen", Email = "burak.sen@hastane.com", PhoneNumber = "0539 567 8901", Biography = "Üroloji uzmanı. Böbrek ve idrar yolu hastalıkları konusunda 10 yıllık deneyim.", PhotoUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=150&h=150&fit=crop&crop=face", SpecializationId = 1, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "burak.sen@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Sibel", LastName = "Koç", Email = "sibel.koc@hastane.com", PhoneNumber = "0540 678 9012", Biography = "Psikiyatri uzmanı. Anksiyete ve depresyon tedavisi konusunda uzman.", PhotoUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=150&h=150&fit=crop&crop=face", SpecializationId = 2, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "sibel.koc@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Emre", LastName = "Türk", Email = "emre.turk@hastane.com", PhoneNumber = "0541 789 0123", Biography = "Kulak Burun Boğaz uzmanı. Sinüzit ve alerji tedavisi konusunda uzman.", PhotoUrl = "https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=150&h=150&fit=crop&crop=face", SpecializationId = 6, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "emre.turk@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Gül", LastName = "Yıldız", Email = "gul.yildiz@hastane.com", PhoneNumber = "0542 890 1234", Biography = "Endokrinoloji uzmanı. Diyabet ve tiroid hastalıkları konusunda uzman.", PhotoUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=150&h=150&fit=crop&crop=face", SpecializationId = 7, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "gul.yildiz@hastane.com", Password = "Doktor123!" },
                    new { Doctor = new Doctor { FirstName = "Dr. Can", LastName = "Aydın", Email = "can.aydin@hastane.com", PhoneNumber = "0543 901 2345", Biography = "Gastroenteroloji uzmanı. Mide ve bağırsak hastalıkları konusunda uzman.", PhotoUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=150&h=150&fit=crop&crop=face", SpecializationId = 7, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }, Email = "can.aydin@hastane.com", Password = "Doktor123!" }
                };

                // Her doktor için kullanıcı hesabı oluştur ve doktoru ekle
                foreach (var data in doctorData)
                {
                    // Önce kullanıcı var mı kontrol et
                    var existingUser = await userManager.FindByEmailAsync(data.Email);
                    IdentityUser doctorUser;
                    
                    if (existingUser == null)
                    {
                        // Kullanıcı hesabı oluştur
                        doctorUser = new IdentityUser
                        {
                            UserName = data.Email,
                            Email = data.Email,
                            EmailConfirmed = true
                        };

                        var userResult = await userManager.CreateAsync(doctorUser, data.Password);
                        if (userResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(doctorUser, "Doctor");
                            Console.WriteLine($"✅ Doktor kullanıcısı oluşturuldu: {data.Email}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Doktor kullanıcısı oluşturulamadı {data.Email}: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                            continue; // Bu doktoru atla
                        }
                    }
                    else
                    {
                        // Kullanıcı zaten var, mevcut kullanıcıyı kullan
                        doctorUser = existingUser;
                        
                        // Eğer Doctor rolü yoksa ekle
                        var roles = await userManager.GetRolesAsync(doctorUser);
                        if (!roles.Contains("Doctor"))
                        {
                            await userManager.AddToRoleAsync(doctorUser, "Doctor");
                            Console.WriteLine($"✅ Doktor rolü eklendi: {data.Email}");
                        }
                        Console.WriteLine($"ℹ️ Mevcut kullanıcı kullanılıyor: {data.Email}");
                    }
                    
                    // Doktor entity'sine UserId ekle
                    data.Doctor.UserId = doctorUser.Id;
                    doctors.Add(data.Doctor);
                }
                
                // Tüm doktorları veritabanına ekle
                if (doctors.Any())
                {
                    context.Doctors.AddRange(doctors);
                    context.SaveChanges();
                    Console.WriteLine($"Toplam {doctors.Count} doktor eklendi");
                }

                // Örnek Çalışma Saatleri Ekle
                var workingHours = new List<WorkingHour>();
                foreach (var doctor in doctors)
                {
                    // Pazartesi - Cuma
                    for (int i = 1; i <= 5; i++)
                    {
                        workingHours.Add(new WorkingHour
                        {
                            DoctorId = doctor.Id,
                            DayOfWeek = (DayOfWeek)i,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(17, 0, 0),
                            IsActive = true
                        });
                    }
                }

                context.WorkingHours.AddRange(workingHours);
                context.SaveChanges();
            }
            
            // Mevcut doktorlar için eksik IdentityUser hesaplarını oluştur
            var allDoctors = await context.Doctors
                .Where(d => d.IsActive && !string.IsNullOrEmpty(d.Email))
                .ToListAsync();
            
            foreach (var doctor in allDoctors)
            {
                // Eğer doktorun UserId'si yoksa veya IdentityUser hesabı yoksa
                if (string.IsNullOrEmpty(doctor.UserId))
                {
                    // IdentityUser hesabı var mı kontrol et
                    var existingUser = await userManager.FindByEmailAsync(doctor.Email);
                    IdentityUser doctorUser;
                    
                    if (existingUser == null)
                    {
                        // Kullanıcı hesabı oluştur
                        doctorUser = new IdentityUser
                        {
                            UserName = doctor.Email,
                            Email = doctor.Email,
                            EmailConfirmed = true
                        };

                        var userResult = await userManager.CreateAsync(doctorUser, "Doktor123!");
                        if (userResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(doctorUser, "Doctor");
                            Console.WriteLine($"✅ Mevcut doktor için kullanıcı hesabı oluşturuldu: {doctor.Email}");
                            
                            // Doktor entity'sine UserId ekle
                            doctor.UserId = doctorUser.Id;
                            context.Doctors.Update(doctor);
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            Console.WriteLine($"❌ Mevcut doktor için kullanıcı hesabı oluşturulamadı {doctor.Email}: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        // Kullanıcı zaten var, doktor entity'sine UserId ekle
                        doctorUser = existingUser;
                        
                        // Eğer Doctor rolü yoksa ekle
                        var roles = await userManager.GetRolesAsync(doctorUser);
                        if (!roles.Contains("Doctor"))
                        {
                            await userManager.AddToRoleAsync(doctorUser, "Doctor");
                            Console.WriteLine($"✅ Mevcut kullanıcıya Doctor rolü eklendi: {doctor.Email}");
                        }
                        
                        doctor.UserId = doctorUser.Id;
                        context.Doctors.Update(doctor);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"ℹ️ Mevcut doktor hesabına UserId eklendi: {doctor.Email}");
                    }
                }
                else
                {
                    // UserId var, IdentityUser'ın var olduğunu ve rolünü kontrol et
                    var existingUser = await userManager.FindByIdAsync(doctor.UserId);
                    if (existingUser != null)
                    {
                        var roles = await userManager.GetRolesAsync(existingUser);
                        if (!roles.Contains("Doctor"))
                        {
                            await userManager.AddToRoleAsync(existingUser, "Doctor");
                            Console.WriteLine($"✅ Doktor rolü eklendi: {doctor.Email}");
                        }
                    }
                    else
                    {
                        // UserId var ama IdentityUser bulunamadı, yeniden oluştur
                        var doctorUser = new IdentityUser
                        {
                            UserName = doctor.Email,
                            Email = doctor.Email,
                            EmailConfirmed = true
                        };

                        var userResult = await userManager.CreateAsync(doctorUser, "Doktor123!");
                        if (userResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(doctorUser, "Doctor");
                            Console.WriteLine($"✅ Silinmiş IdentityUser için yeniden hesap oluşturuldu: {doctor.Email}");
                            
                            doctor.UserId = doctorUser.Id;
                            context.Doctors.Update(doctor);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }

            // Test hastaları ekle (sadece hasta kullanıcısı yoksa)
            var testPatients = new List<Patient>();
            if (existingTestPatient == null)
            {
                var testPatientForDb = await userManager.FindByEmailAsync("hasta@test.com");
                testPatients.AddRange(new Patient[]
                {
                    new Patient
                    {
                        FirstName = "Test",
                        LastName = "Hasta",
                        Email = "hasta@test.com",
                        PhoneNumber = "0555 000 0001",
                        TcNumber = "11111111111",
                        BirthDate = new DateTime(1990, 1, 1),
                        Address = "Test Adres",
                        PhotoUrl = "https://images.unsplash.com/photo-1494790108755-2616b612b786?w=200&h=200&fit=crop&crop=face",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1),
                        UserId = testPatientForDb?.Id
                    },
                    new Patient
                    {
                        FirstName = "Ahmet",
                        LastName = "Yılmaz",
                        Email = "ahmet.yilmaz@test.com",
                        PhoneNumber = "0555 000 0002",
                        TcNumber = "22222222222",
                        BirthDate = new DateTime(1985, 5, 15),
                        Address = "İstanbul, Kadıköy",
                        PhotoUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200&h=200&fit=crop&crop=face",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Patient
                    {
                        FirstName = "Ayşe",
                        LastName = "Demir",
                        Email = "ayse.demir@test.com",
                        PhoneNumber = "0555 000 0003",
                        TcNumber = "33333333333",
                        BirthDate = new DateTime(1992, 8, 22),
                        Address = "Ankara, Çankaya",
                        PhotoUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=200&h=200&fit=crop&crop=face",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Patient
                    {
                        FirstName = "Mehmet",
                        LastName = "Kaya",
                        Email = "mehmet.kaya@test.com",
                        PhoneNumber = "0555 000 0004",
                        TcNumber = "44444444444",
                        BirthDate = new DateTime(1978, 12, 3),
                        Address = "İzmir, Konak",
                        PhotoUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=200&h=200&fit=crop&crop=face",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    },
                    new Patient
                    {
                        FirstName = "Fatma",
                        LastName = "Şahin",
                        Email = "fatma.sahin@test.com",
                        PhoneNumber = "0555 000 0005",
                        TcNumber = "55555555555",
                        BirthDate = new DateTime(1988, 3, 10),
                        Address = "Bursa, Osmangazi",
                        PhotoUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=200&h=200&fit=crop&crop=face",
                        IsActive = true,
                        CreatedDate = new DateTime(2024, 1, 1)
                    }
                });

                context.Patients.AddRange(testPatients);
                context.SaveChanges();
                Console.WriteLine($"Test hastaları oluşturuldu: {testPatients.Count} hasta");
            }

            // Örnek Randevular Ekle - Test doktoru için çok sayıda randevu
            var appointments = new List<Appointment>();
            
            // Test doktoru (ID: 1) için randevular
            if (testPatients.Count > 0)
            {
                appointments.AddRange(new Appointment[]
                {
                    // Bugünkü randevular
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[0].Id,
                        AppointmentDate = DateTime.Today,
                        AppointmentTime = new TimeSpan(9, 0, 0),
                        Notes = "Kontrol muayenesi - Kalp ritmi kontrolü",
                        Status = AppointmentStatus.Confirmed,
                        CreatedDate = DateTime.Now.AddDays(-1)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[1].Id,
                        AppointmentDate = DateTime.Today,
                        AppointmentTime = new TimeSpan(10, 30, 0),
                        Notes = "EKG çekimi ve değerlendirme",
                        Status = AppointmentStatus.Confirmed,
                        CreatedDate = DateTime.Now.AddDays(-2)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[2].Id,
                        AppointmentDate = DateTime.Today,
                        AppointmentTime = new TimeSpan(14, 0, 0),
                        Notes = "Göğüs ağrısı şikayeti - Detaylı muayene",
                        Status = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.Now.AddDays(-1)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[3].Id,
                        AppointmentDate = DateTime.Today,
                        AppointmentTime = new TimeSpan(15, 30, 0),
                        Notes = "Hipertansiyon takibi - Tansiyon ölçümü",
                        Status = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.Now.AddDays(-3)
                    },
                    
                    // Geçmiş randevular (tamamlanan)
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[0].Id,
                        AppointmentDate = DateTime.Today.AddDays(-1),
                        AppointmentTime = new TimeSpan(10, 0, 0),
                        Notes = "EKG çekimi tamamlandı - Normal sonuç",
                        Status = AppointmentStatus.Completed,
                        CreatedDate = DateTime.Now.AddDays(-5)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[1].Id,
                        AppointmentDate = DateTime.Today.AddDays(-2),
                        AppointmentTime = new TimeSpan(11, 0, 0),
                        Notes = "Holter monitörizasyonu - 24 saatlik takip",
                        Status = AppointmentStatus.Completed,
                        CreatedDate = DateTime.Now.AddDays(-7)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[2].Id,
                        AppointmentDate = DateTime.Today.AddDays(-3),
                        AppointmentTime = new TimeSpan(14, 30, 0),
                        Notes = "Ekokardiyografi - Kalp ultrasonu",
                        Status = AppointmentStatus.Completed,
                        CreatedDate = DateTime.Now.AddDays(-10)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[3].Id,
                        AppointmentDate = DateTime.Today.AddDays(-5),
                        AppointmentTime = new TimeSpan(9, 30, 0),
                        Notes = "Stres testi - Egzersiz testi",
                        Status = AppointmentStatus.Completed,
                        CreatedDate = DateTime.Now.AddDays(-12)
                    },
                    
                    // Gelecek randevular
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[0].Id,
                        AppointmentDate = DateTime.Today.AddDays(1),
                        AppointmentTime = new TimeSpan(9, 30, 0),
                        Notes = "Kontrol muayenesi - İlaç doz ayarı",
                        Status = AppointmentStatus.Confirmed,
                        CreatedDate = DateTime.Now.AddDays(-2)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[1].Id,
                        AppointmentDate = DateTime.Today.AddDays(1),
                        AppointmentTime = new TimeSpan(11, 0, 0),
                        Notes = "Koroner anjiyografi öncesi değerlendirme",
                        Status = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.Now.AddDays(-1)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[2].Id,
                        AppointmentDate = DateTime.Today.AddDays(2),
                        AppointmentTime = new TimeSpan(10, 0, 0),
                        Notes = "Kalp pili kontrolü - Programlama",
                        Status = AppointmentStatus.Confirmed,
                        CreatedDate = DateTime.Now.AddDays(-3)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[3].Id,
                        AppointmentDate = DateTime.Today.AddDays(2),
                        AppointmentTime = new TimeSpan(14, 0, 0),
                        Notes = "Aritmi takibi - Holter sonuçları",
                        Status = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.Now.AddDays(-4)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[4].Id,
                        AppointmentDate = DateTime.Today.AddDays(3),
                        AppointmentTime = new TimeSpan(9, 0, 0),
                        Notes = "İlk muayene - Kalp şikayetleri",
                        Status = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.Now.AddDays(-2)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[0].Id,
                        AppointmentDate = DateTime.Today.AddDays(3),
                        AppointmentTime = new TimeSpan(15, 0, 0),
                        Notes = "Koroner bypass sonrası kontrol",
                        Status = AppointmentStatus.Confirmed,
                        CreatedDate = DateTime.Now.AddDays(-5)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[1].Id,
                        AppointmentDate = DateTime.Today.AddDays(4),
                        AppointmentTime = new TimeSpan(10, 30, 0),
                        Notes = "Kalp krizi sonrası rehabilitasyon",
                        Status = AppointmentStatus.Scheduled,
                        CreatedDate = DateTime.Now.AddDays(-3)
                    },
                    new Appointment
                    {
                        DoctorId = 1,
                        PatientId = testPatients[2].Id,
                        AppointmentDate = DateTime.Today.AddDays(5),
                        AppointmentTime = new TimeSpan(11, 30, 0),
                        Notes = "Kalp yetmezliği takibi - İlaç ayarı",
                        Status = AppointmentStatus.Confirmed,
                        CreatedDate = DateTime.Now.AddDays(-4)
                    }
                });
            }
            
            // Diğer doktorlar için de bazı randevular ekle
            appointments.AddRange(new Appointment[]
            {
                new Appointment
                {
                    DoctorId = 2,
                    PatientId = testPatients.Count > 0 ? testPatients[0].Id : 1,
                    AppointmentDate = DateTime.Today.AddDays(1),
                    AppointmentTime = new TimeSpan(14, 0, 0),
                    Notes = "Aşı kontrolü - 6 aylık kontrol",
                    Status = AppointmentStatus.Confirmed,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Appointment
                {
                    DoctorId = 3,
                    PatientId = testPatients.Count > 1 ? testPatients[1].Id : 1,
                    AppointmentDate = DateTime.Today.AddDays(2),
                    AppointmentTime = new TimeSpan(9, 30, 0),
                    Notes = "Diz ağrısı şikayeti - Röntgen gerekebilir",
                    Status = AppointmentStatus.Scheduled,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Appointment
                {
                    DoctorId = 4,
                    PatientId = testPatients.Count > 2 ? testPatients[2].Id : 1,
                    AppointmentDate = DateTime.Today.AddDays(3),
                    AppointmentTime = new TimeSpan(11, 0, 0),
                    Notes = "Hamilelik takibi - 3. ay kontrolü",
                    Status = AppointmentStatus.Confirmed,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            });

            context.Appointments.AddRange(appointments);
            context.SaveChanges();
        }
    }
}
