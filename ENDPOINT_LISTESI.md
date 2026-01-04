# 📋 PROJE ENDPOINT LİSTESİ

## 🔌 API ENDPOINT'LERİ (JSON Response)

### Home Controller API'leri

| Method | Endpoint | Açıklama | Response |
|--------|----------|----------|----------|
| **GET** | `/Home/GetNotifications` | Okunmamış bildirimleri getir | `{ success, notifications[], unreadCount }` |
| **POST** | `/Home/MarkNotificationAsRead` | Bildirimi okundu işaretle | `{ success }` |
| **POST** | `/Home/MarkAllNotificationsAsRead` | Tüm bildirimleri okundu işaretle | `{ success }` |

### Doctor Controller API'leri

| Method | Endpoint | Parametreler | Açıklama | Response |
|--------|----------|--------------|----------|----------|
| **GET** | `/Doctor/GetAvailableDates` | `?doctorId={id}` | Doktorun müsait tarihlerini getir | `["2025-01-20", "2025-01-21", ...]` |
| **GET** | `/Doctor/GetAvailableTimeSlots` | `?doctorId={id}&date={date}` | Belirli tarih için müsait saatleri getir | `{ success, slots[] }` |

---

## 🏠 HOME CONTROLLER

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| **GET** | `/` veya `/Home/Index` | Ana sayfa |
| **GET** | `/Home/Dashboard` | Dashboard (istatistikler) |
| **GET** | `/Home/Privacy` | Gizlilik sayfası |
| **GET** | `/Home/Error` | Hata sayfası |

---

## 👨‍⚕️ DOCTOR CONTROLLER

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| **GET** | `/Doctor` | Doktor listesi | Herkes |
| **GET** | `/Doctor/Details/{id}` | Doktor detayları | Herkes |
| **GET** | `/Doctor/Panel` | Doktor paneli | Doctor |
| **GET** | `/Doctor/Create` | Doktor oluşturma formu | - |
| **POST** | `/Doctor/Create` | Doktor oluştur | - |
| **GET** | `/Doctor/Edit/{id}` | Doktor düzenleme formu | - |
| **POST** | `/Doctor/Edit/{id}` | Doktor güncelle | - |
| **GET** | `/Doctor/Delete/{id}` | Doktor silme onayı | - |
| **POST** | `/Doctor/Delete/{id}` | Doktor sil | - |
| **GET** | `/Doctor/Appointments/{id}` | Doktorun randevuları | Herkes |
| **GET** | `/Doctor/Availability/{id}` | Doktor müsaitlik sayfası | Herkes |
| **GET** | `/Doctor/Book/{id}` | Doktor için randevu alma | Herkes |
| **POST** | `/Doctor/Book` | Randevu oluştur | Herkes |
| **POST** | `/Doctor/ApproveAppointment/{id}` | Randevu onayla | Doctor |
| **POST** | `/Doctor/RejectAppointment/{id}` | Randevu reddet | Doctor |
| **POST** | `/Doctor/ConfirmAppointment/{id}` | Randevu onayla (alternatif) | Doctor |
| **POST** | `/Doctor/CancelAppointment/{id}` | Randevu iptal et | Doctor |
| **POST** | `/Doctor/CompleteAppointment/{id}` | Randevu tamamla | Doctor |

---

## 📅 APPOINTMENT CONTROLLER

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| **GET** | `/Appointment` | Randevu listesi (hastanın kendi randevuları) | Patient |
| **GET** | `/Appointment/Details/{id}` | Randevu detayları | - |
| **GET** | `/Appointment/Create` | Randevu oluşturma formu | Patient |
| **POST** | `/Appointment/Create` | Randevu oluştur | Patient |
| **GET** | `/Appointment/Edit/{id}` | Randevu düzenleme formu | - |
| **POST** | `/Appointment/Edit/{id}` | Randevu güncelle | - |
| **GET** | `/Appointment/Delete/{id}` | Randevu silme onayı | - |
| **POST** | `/Appointment/Delete/{id}` | Randevu sil | - |
| **GET** | `/Appointment/Book` | Randevu alma sayfası | Herkes |
| **POST** | `/Appointment/Book` | Randevu oluştur | Herkes |
| **POST** | `/Appointment/Approve/{id}` | Randevu onayla | Doctor |
| **POST** | `/Appointment/Reject/{id}` | Randevu reddet | Doctor |
| **POST** | `/Appointment/Complete/{id}` | Randevu tamamla | Doctor |

---

## 👤 PATIENT CONTROLLER

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| **GET** | `/Patient` | Hasta listesi | - |
| **GET** | `/Patient/Details/{id}` | Hasta detayları | - |
| **GET** | `/Patient/Panel` | Hasta paneli | Patient |
| **GET** | `/Patient/Create` | Hasta oluşturma formu | - |
| **POST** | `/Patient/Create` | Hasta oluştur | - |
| **GET** | `/Patient/Edit/{id}` | Hasta düzenleme formu | - |
| **POST** | `/Patient/Edit/{id}` | Hasta güncelle | - |
| **GET** | `/Patient/Delete/{id}` | Hasta silme onayı | - |
| **POST** | `/Patient/Delete/{id}` | Hasta sil | - |

---

## 🏥 SPECIALIZATION CONTROLLER

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| **GET** | `/Specialization` | Branş listesi |
| **GET** | `/Specialization/Details/{id}` | Branş detayları |
| **GET** | `/Specialization/Create` | Branş oluşturma formu |
| **POST** | `/Specialization/Create` | Branş oluştur |
| **GET** | `/Specialization/Edit/{id}` | Branş düzenleme formu |
| **POST** | `/Specialization/Edit/{id}` | Branş güncelle |
| **GET** | `/Specialization/Delete/{id}` | Branş silme onayı |
| **POST** | `/Specialization/Delete/{id}` | Branş sil |

---

## ⏰ WORKING HOUR CONTROLLER

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| **GET** | `/WorkingHour` | Çalışma saatleri listesi | Doctor |
| **GET** | `/WorkingHour/Details/{id}` | Çalışma saati detayları | - |
| **GET** | `/WorkingHour/Create` | Çalışma saati oluşturma formu | Doctor |
| **POST** | `/WorkingHour/Create` | Çalışma saati oluştur | Doctor |
| **GET** | `/WorkingHour/Edit/{id}` | Çalışma saati düzenleme formu | - |
| **POST** | `/WorkingHour/Edit/{id}` | Çalışma saati güncelle | - |
| **GET** | `/WorkingHour/Delete/{id}` | Çalışma saati silme onayı | - |
| **POST** | `/WorkingHour/Delete/{id}` | Çalışma saati sil | - |

---

## 🚫 AVAILABILITY BLOCK CONTROLLER

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| **GET** | `/AvailabilityBlock` | Müsaitlik blokları listesi | Doctor |
| **GET** | `/AvailabilityBlock/Details/{id}` | Müsaitlik bloku detayları | - |
| **GET** | `/AvailabilityBlock/Create` | Müsaitlik bloku oluşturma formu | Doctor |
| **POST** | `/AvailabilityBlock/Create` | Müsaitlik bloku oluştur | Doctor |
| **GET** | `/AvailabilityBlock/Edit/{id}` | Müsaitlik bloku düzenleme formu | - |
| **POST** | `/AvailabilityBlock/Edit/{id}` | Müsaitlik bloku güncelle | - |
| **GET** | `/AvailabilityBlock/Delete/{id}` | Müsaitlik bloku silme onayı | - |
| **POST** | `/AvailabilityBlock/Delete/{id}` | Müsaitlik bloku sil | - |

---

## 🧪 TEST CONTROLLER

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| **GET** | `/Test/CreateTestUser` | Test kullanıcısı oluştur |
| **GET** | `/Test/ListUsers` | Kullanıcı listesi |
| **GET** | `/Test/QuickTest` | Hızlı test (kullanıcıları sıfırla ve oluştur) |
| **GET** | `/Test/ResetUsers` | Kullanıcıları sıfırla |

---

## 🔐 IDENTITY ENDPOINT'LERİ

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| **GET** | `/Identity/Account/Login` | Giriş sayfası |
| **POST** | `/Identity/Account/Login` | Giriş yap |
| **GET** | `/Identity/Account/Register` | Kayıt sayfası |
| **POST** | `/Identity/Account/Register` | Kayıt ol |
| **GET** | `/Identity/Account/Logout` | Çıkış sayfası |
| **POST** | `/Identity/Account/Logout` | Çıkış yap |
| **GET** | `/Identity/Account/RegisterConfirmation` | Kayıt onay sayfası |
| **GET** | `/Identity/Account/ConfirmEmail` | E-posta onay |

---

## 📊 ÖZET İSTATİSTİKLER

- **Toplam Endpoint:** 70+
- **API Endpoint'leri (JSON):** 5
- **MVC Endpoint'leri (View):** 65+
- **CRUD İşlemleri:** 7 Controller (Doctor, Patient, Appointment, Specialization, WorkingHour, AvailabilityBlock)
- **Panel Sayfaları:** 2 (Doctor/Panel, Patient/Panel)
- **Test Endpoint'leri:** 4

---

## 🎯 ÖNEMLİ ENDPOINT'LER

### Randevu İşlemleri:
- `GET /Appointment/Create` - Randevu oluşturma
- `POST /Appointment/Create` - Randevu kaydetme
- `GET /Appointment` - Randevu listesi (hasta)
- `GET /Doctor/Panel` - Randevu listesi (doktor)

### API Endpoint'leri:
- `GET /Doctor/GetAvailableDates?doctorId={id}` - Müsait tarihler
- `GET /Doctor/GetAvailableTimeSlots?doctorId={id}&date={date}` - Müsait saatler
- `GET /Home/GetNotifications` - Bildirimler

### Panel Sayfaları:
- `GET /Doctor/Panel` - Doktor paneli
- `GET /Patient/Panel` - Hasta paneli

---

## 💡 KULLANIM ÖRNEKLERİ

### API Kullanımı:
```javascript
// Müsait tarihleri getir
fetch('/Doctor/GetAvailableDates?doctorId=1')
  .then(response => response.json())
  .then(data => console.log(data));

// Müsait saatleri getir
fetch('/Doctor/GetAvailableTimeSlots?doctorId=1&date=2025-01-20')
  .then(response => response.json())
  .then(data => console.log(data));

// Bildirimleri getir
fetch('/Home/GetNotifications')
  .then(response => response.json())
  .then(data => console.log(data));
```

### MVC Kullanımı:
```
http://localhost:5090/Doctor/Panel
http://localhost:5090/Patient/Panel
http://localhost:5090/Appointment/Create
```

---

**NOT:** Tüm endpoint'ler projede mevcut ve çalışır durumda! 🚀

