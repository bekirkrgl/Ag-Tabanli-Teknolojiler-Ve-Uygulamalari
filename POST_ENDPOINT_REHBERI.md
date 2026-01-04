# 📤 POST ENDPOINT'LERİ REHBERİ

## 🔍 POST Endpoint'lerini Nerede Bulabilirsiniz?

### ✅ Yöntem 1: Controller Dosyalarında `[HttpPost]` Attribute'ü ile Arama

**Visual Studio'da:**
1. `Ctrl+Shift+F` (Tüm dosyalarda arama)
2. Arama kutusuna: `[HttpPost]` yaz
3. Sonuçları göster

**Veya:**
- Her Controller dosyasını aç
- `Ctrl+F` ile `[HttpPost]` ara
- POST metodları bulunur

---

## 📋 TÜM POST ENDPOINT'LERİ

### 🏠 HOME CONTROLLER
**Dosya:** `Controllers/HomeController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Home/MarkNotificationAsRead` | 90 | `MarkNotificationAsRead([FromBody] int id)` |
| `POST /Home/MarkAllNotificationsAsRead` | 106 | `MarkAllNotificationsAsRead()` |

**Kod Örneği:**
```csharp
// Satır 90
[HttpPost]
public async Task<IActionResult> MarkNotificationAsRead([FromBody] int id)
{
    // ...
    return Json(new { success = true });
}
```

---

### 👨‍⚕️ DOCTOR CONTROLLER
**Dosya:** `Controllers/DoctorController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Doctor/Create` | 175 | `Create([Bind(...)] Doctor doctor)` |
| `POST /Doctor/Edit/{id}` | 208 | `Edit(int id, [Bind(...)] Doctor doctor)` |
| `POST /Doctor/Delete/{id}` | 261 | `DeleteConfirmed(int id)` |
| `POST /Doctor/Book` | 400 | `Book([Bind(...)] Appointment appointment)` |
| `POST /Doctor/ApproveAppointment/{id}` | 439 | `ApproveAppointment(int id)` |
| `POST /Doctor/RejectAppointment/{id}` | 480 | `RejectAppointment(int id)` |
| `POST /Doctor/ConfirmAppointment/{id}` | 521 | `ConfirmAppointment(int? id)` |
| `POST /Doctor/CancelAppointment/{id}` | 586 | `CancelAppointment(int? id)` |
| `POST /Doctor/CompleteAppointment/{id}` | 651 | `CompleteAppointment(int id)` |

**Kod Örneği:**
```csharp
// Satır 175
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("FirstName,LastName,...")] Doctor doctor)
{
    // ...
}
```

---

### 📅 APPOINTMENT CONTROLLER
**Dosya:** `Controllers/AppointmentController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Appointment/Create` | 171 | `Create([Bind("DoctorId,Notes")] Appointment appointment)` |
| `POST /Appointment/Edit/{id}` | 506 | `Edit(int id, [Bind(...)] Appointment appointment)` |
| `POST /Appointment/Delete/{id}` | 578 | `DeleteConfirmed(int id)` |
| `POST /Appointment/Book` | 799 | `Book([Bind(...)] Appointment appointment)` |
| `POST /Appointment/Approve/{id}` | 596 | `Approve(int id)` |
| `POST /Appointment/Reject/{id}` | 644 | `Reject(int id)` |
| `POST /Appointment/Complete/{id}` | 692 | `Complete(int id)` |

**Kod Örneği:**
```csharp
// Satır 171
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("DoctorId,Notes")] Appointment appointment)
{
    // ...
}
```

---

### 👤 PATIENT CONTROLLER
**Dosya:** `Controllers/PatientController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Patient/Create` | 67 | `Create([Bind(...)] Patient patient)` |
| `POST /Patient/Edit/{id}` | 98 | `Edit(int id, [Bind(...)] Patient patient)` |
| `POST /Patient/Delete/{id}` | 152 | `DeleteConfirmed(int id)` |

---

### 🏥 SPECIALIZATION CONTROLLER
**Dosya:** `Controllers/SpecializationController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Specialization/Create` | 53 | `Create([Bind("Id,Name,Description")] Specialization specialization)` |
| `POST /Specialization/Edit/{id}` | 83 | `Edit(int id, [Bind(...)] Specialization specialization)` |
| `POST /Specialization/Delete/{id}` | 138 | `DeleteConfirmed(int id)` |

---

### ⏰ WORKING HOUR CONTROLLER
**Dosya:** `Controllers/WorkingHourController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /WorkingHour/Create` | 96 | `Create([Bind(...)] WorkingHour workingHour)` |
| `POST /WorkingHour/Edit/{id}` | 153 | `Edit(int id, [Bind(...)] WorkingHour workingHour)` |
| `POST /WorkingHour/Delete/{id}` | 217 | `DeleteConfirmed(int id)` |

---

### 🚫 AVAILABILITY BLOCK CONTROLLER
**Dosya:** `Controllers/AvailabilityBlockController.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /AvailabilityBlock/Create` | 95 | `Create([Bind(...)] AvailabilityBlock availabilityBlock)` |
| `POST /AvailabilityBlock/Edit/{id}` | 153 | `Edit(int id, [Bind(...)] AvailabilityBlock availabilityBlock)` |
| `POST /AvailabilityBlock/Delete/{id}` | 217 | `DeleteConfirmed(int id)` |

---

### 🔐 IDENTITY POST ENDPOINT'LERİ
**Dosya:** `Areas/Identity/Pages/Account/Login.cshtml.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Identity/Account/Login` | 118 | `OnPostAsync(string returnUrl, string role)` |

**Dosya:** `Areas/Identity/Pages/Account/Register.cshtml.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Identity/Account/Register` | (Register.cshtml.cs içinde) | `OnPostAsync(...)` |

**Dosya:** `Areas/Identity/Pages/Account/Logout.cshtml.cs`

| Endpoint | Satır | Metod İmzası |
|----------|-------|--------------|
| `POST /Identity/Account/Logout` | 26 | `OnPost(string returnUrl)` |

---

## 🔍 POST ENDPOINT'LERİNİ BULMA YÖNTEMLERİ

### Yöntem 1: Visual Studio'da Arama
```
1. Ctrl+Shift+F (Tüm dosyalarda arama)
2. Arama: [HttpPost]
3. Klasör: Controllers/
4. Sonuçları göster
```

### Yöntem 2: Her Controller Dosyasında Arama
```
1. Controller dosyasını aç (örn: AppointmentController.cs)
2. Ctrl+F
3. Arama: [HttpPost]
4. Sonraki eşleşmeye git (F3)
```

### Yöntem 3: Metod İmzasına Göre Arama
POST metodları genelde şu pattern'i takip eder:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MethodName(...)
```

Arama: `[HttpPost]` veya `ValidateAntiForgeryToken`

---

## 📊 POST ENDPOINT İSTATİSTİKLERİ

- **Toplam POST Endpoint:** 30+
- **CRUD POST'ları:** 21 (Create, Edit, Delete)
- **İş Mantığı POST'ları:** 9+ (Approve, Reject, Complete, vb.)
- **Identity POST'ları:** 3 (Login, Register, Logout)

---

## 💡 POST ENDPOINT ÖZELLİKLERİ

### Ortak Özellikler:
1. **`[HttpPost]` Attribute:** HTTP POST metodunu belirtir
2. **`[ValidateAntiForgeryToken]`:** CSRF koruması
3. **`[Bind(...)]`:** Model binding (hangi property'lerin bağlanacağını belirtir)
4. **Form Validation:** `ModelState.IsValid` kontrolü

### Örnek POST Metodu:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("DoctorId,Notes")] Appointment appointment)
{
    if (ModelState.IsValid)
    {
        // İşlem yap
        _context.Add(appointment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(appointment);
}
```

---

## 🎯 HIZLI ERİŞİM

### En Çok Kullanılan POST Endpoint'ler:

1. **Randevu Oluşturma:**
   - `POST /Appointment/Create` → `AppointmentController.cs` satır 171

2. **Randevu Onaylama:**
   - `POST /Appointment/Approve/{id}` → `AppointmentController.cs` satır 596
   - `POST /Doctor/ApproveAppointment/{id}` → `DoctorController.cs` satır 439

3. **Kullanıcı Girişi:**
   - `POST /Identity/Account/Login` → `Login.cshtml.cs` satır 118

4. **Bildirim İşlemleri:**
   - `POST /Home/MarkNotificationAsRead` → `HomeController.cs` satır 90

---

## 📂 DOSYA KONUMLARI ÖZET

```
Controllers/
├── HomeController.cs              → 2 POST endpoint
├── DoctorController.cs            → 9 POST endpoint
├── AppointmentController.cs       → 7 POST endpoint
├── PatientController.cs            → 3 POST endpoint
├── SpecializationController.cs    → 3 POST endpoint
├── WorkingHourController.cs       → 3 POST endpoint
└── AvailabilityBlockController.cs → 3 POST endpoint

Areas/Identity/Pages/Account/
├── Login.cshtml.cs                → 1 POST endpoint
├── Register.cshtml.cs             → 1 POST endpoint
└── Logout.cshtml.cs               → 1 POST endpoint
```

---

**NOT:** Tüm POST endpoint'leri `[HttpPost]` attribute'ü ile işaretlenmiştir! 🔍

