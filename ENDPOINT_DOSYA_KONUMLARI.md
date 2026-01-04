# 📁 ENDPOINT'LERİN DOSYA KONUMLARI

## 🔍 Endpoint'leri Nerede Bulabilirsiniz?

### 📂 Controller Dosyaları (Ana Konum)

Tüm endpoint'ler **Controllers** klasöründeki dosyalarda tanımlıdır:

```
HastaneRandevuSistemi/
└── Controllers/
    ├── HomeController.cs          → Ana sayfa ve bildirim API'leri
    ├── DoctorController.cs        → Doktor işlemleri ve API'leri
    ├── AppointmentController.cs   → Randevu işlemleri
    ├── PatientController.cs       → Hasta işlemleri
    ├── SpecializationController.cs → Branş işlemleri
    ├── WorkingHourController.cs   → Çalışma saatleri
    ├── AvailabilityBlockController.cs → Müsaitlik blokları
    └── TestController.cs          → Test endpoint'leri
```

---

## 📋 ENDPOINT'LERİN DETAYLI KONUMLARI

### 🏠 HOME CONTROLLER
**Dosya:** `Controllers/HomeController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /` | 23 | `Index()` |
| `GET /Home/Dashboard` | 38 | `Dashboard()` |
| `GET /Home/Privacy` | 54 | `Privacy()` |
| `GET /Home/GetNotifications` | 60 | `GetNotifications()` |
| `POST /Home/MarkNotificationAsRead` | 90 | `MarkNotificationAsRead()` |
| `POST /Home/MarkAllNotificationsAsRead` | 106 | `MarkAllNotificationsAsRead()` |
| `GET /Home/Error` | 122 | `Error()` |

---

### 👨‍⚕️ DOCTOR CONTROLLER
**Dosya:** `Controllers/DoctorController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /Doctor` | 28 | `Index()` |
| `GET /Doctor/Details/{id}` | 38 | `Details(int? id)` |
| `GET /Doctor/Panel` | 60 | `Panel()` |
| `GET /Doctor/Create` | 168 | `Create()` |
| `POST /Doctor/Create` | 175 | `Create([Bind(...)] Doctor doctor)` |
| `GET /Doctor/Edit/{id}` | 191 | `Edit(int? id)` |
| `POST /Doctor/Edit/{id}` | 208 | `Edit(int id, [Bind(...)] Doctor doctor)` |
| `GET /Doctor/Delete/{id}` | 242 | `Delete(int? id)` |
| `POST /Doctor/Delete/{id}` | 261 | `DeleteConfirmed(int id)` |
| `GET /Doctor/Appointments/{id}` | 282 | `Appointments(int? id)` |
| `GET /Doctor/Availability/{id}` | 309 | `Availability(int? id)` |
| `GET /Doctor/GetAvailableDates` | 336 | `GetAvailableDates(int? doctorId)` ⭐ API |
| `GET /Doctor/GetAvailableTimeSlots` | 349 | `GetAvailableTimeSlots(int? doctorId, string date)` ⭐ API |
| `GET /Doctor/Book/{id}` | 373 | `Book(int? id)` |
| `POST /Doctor/Book` | 400 | `Book([Bind(...)] Appointment appointment)` |
| `POST /Doctor/ApproveAppointment/{id}` | 439 | `ApproveAppointment(int id)` |
| `POST /Doctor/RejectAppointment/{id}` | 480 | `RejectAppointment(int id)` |
| `POST /Doctor/ConfirmAppointment/{id}` | 521 | `ConfirmAppointment(int? id)` |
| `POST /Doctor/CancelAppointment/{id}` | 586 | `CancelAppointment(int? id)` |
| `POST /Doctor/CompleteAppointment/{id}` | 651 | `CompleteAppointment(int id)` |

---

### 📅 APPOINTMENT CONTROLLER
**Dosya:** `Controllers/AppointmentController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /Appointment` | 22 | `Index()` |
| `GET /Appointment/Details/{id}` | 60 | `Details(int? id)` |
| `GET /Appointment/Create` | 82 | `Create(int? doctorId)` |
| `POST /Appointment/Create` | 171 | `Create([Bind(...)] Appointment appointment)` |
| `GET /Appointment/Edit/{id}` | 476 | `Edit(int? id)` |
| `POST /Appointment/Edit/{id}` | 506 | `Edit(int id, [Bind(...)] Appointment appointment)` |
| `GET /Appointment/Delete/{id}` | 554 | `Delete(int? id)` |
| `POST /Appointment/Delete/{id}` | 578 | `DeleteConfirmed(int id)` |
| `GET /Appointment/Book` | 739 | `Book(int? doctorId)` |
| `POST /Appointment/Book` | 799 | `Book([Bind(...)] Appointment appointment)` |
| `POST /Appointment/Approve/{id}` | 596 | `Approve(int id)` |
| `POST /Appointment/Reject/{id}` | 644 | `Reject(int id)` |
| `POST /Appointment/Complete/{id}` | 692 | `Complete(int id)` |

---

### 👤 PATIENT CONTROLLER
**Dosya:** `Controllers/PatientController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /Patient` | 24 | `Index()` |
| `GET /Patient/Details/{id}` | 33 | `Details(int? id)` |
| `GET /Patient/Panel` | 171 | `Panel(string? specialization, string? search)` |
| `GET /Patient/Create` | 61 | `Create()` |
| `POST /Patient/Create` | 67 | `Create([Bind(...)] Patient patient)` |
| `GET /Patient/Edit/{id}` | 82 | `Edit(int? id)` |
| `POST /Patient/Edit/{id}` | 98 | `Edit(int id, [Bind(...)] Patient patient)` |
| `GET /Patient/Delete/{id}` | 131 | `Delete(int? id)` |
| `POST /Patient/Delete/{id}` | 152 | `DeleteConfirmed(int id)` |

---

### 🏥 SPECIALIZATION CONTROLLER
**Dosya:** `Controllers/SpecializationController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /Specialization` | 18 | `Index()` |
| `GET /Specialization/Details/{id}` | 27 | `Details(int? id)` |
| `GET /Specialization/Create` | 47 | `Create()` |
| `POST /Specialization/Create` | 53 | `Create([Bind(...)] Specialization specialization)` |
| `GET /Specialization/Edit/{id}` | 67 | `Edit(int? id)` |
| `POST /Specialization/Edit/{id}` | 83 | `Edit(int id, [Bind(...)] Specialization specialization)` |
| `GET /Specialization/Delete/{id}` | 116 | `Delete(int? id)` |
| `POST /Specialization/Delete/{id}` | 138 | `DeleteConfirmed(int id)` |

---

### ⏰ WORKING HOUR CONTROLLER
**Dosya:** `Controllers/WorkingHourController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /WorkingHour` | 21 | `Index()` |
| `GET /WorkingHour/Details/{id}` | 51 | `Details(int? id)` |
| `GET /WorkingHour/Create` | 72 | `Create()` |
| `POST /WorkingHour/Create` | 96 | `Create([Bind(...)] WorkingHour workingHour)` |
| `GET /WorkingHour/Edit/{id}` | 129 | `Edit(int? id)` |
| `POST /WorkingHour/Edit/{id}` | 153 | `Edit(int id, [Bind(...)] WorkingHour workingHour)` |
| `GET /WorkingHour/Delete/{id}` | 194 | `Delete(int? id)` |
| `POST /WorkingHour/Delete/{id}` | 217 | `DeleteConfirmed(int id)` |

---

### 🚫 AVAILABILITY BLOCK CONTROLLER
**Dosya:** `Controllers/AvailabilityBlockController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /AvailabilityBlock` | 21 | `Index()` |
| `GET /AvailabilityBlock/Details/{id}` | 50 | `Details(int? id)` |
| `GET /AvailabilityBlock/Create` | 71 | `Create()` |
| `POST /AvailabilityBlock/Create` | 95 | `Create([Bind(...)] AvailabilityBlock availabilityBlock)` |
| `GET /AvailabilityBlock/Edit/{id}` | 129 | `Edit(int? id)` |
| `POST /AvailabilityBlock/Edit/{id}` | 153 | `Edit(int id, [Bind(...)] AvailabilityBlock availabilityBlock)` |
| `GET /AvailabilityBlock/Delete/{id}` | 194 | `Delete(int? id)` |
| `POST /AvailabilityBlock/Delete/{id}` | 217 | `DeleteConfirmed(int id)` |

---

### 🧪 TEST CONTROLLER
**Dosya:** `Controllers/TestController.cs`

| Endpoint | Satır | Metod Adı |
|----------|-------|-----------|
| `GET /Test/CreateTestUser` | 19 | `CreateTestUser()` |
| `GET /Test/ListUsers` | 80 | `ListUsers()` |
| `GET /Test/QuickTest` | 111 | `QuickTest()` |
| `GET /Test/ResetUsers` | 208 | `ResetUsers()` |

---

## 🔐 IDENTITY ENDPOINT'LERİ

**Klasör:** `Areas/Identity/Pages/Account/`

| Endpoint | Dosya |
|----------|-------|
| `GET /Identity/Account/Login` | `Areas/Identity/Pages/Account/Login.cshtml.cs` → `OnGetAsync()` |
| `POST /Identity/Account/Login` | `Areas/Identity/Pages/Account/Login.cshtml.cs` → `OnPostAsync()` |
| `GET /Identity/Account/Register` | `Areas/Identity/Pages/Account/Register.cshtml.cs` → `OnGetAsync()` |
| `POST /Identity/Account/Register` | `Areas/Identity/Pages/Account/Register.cshtml.cs` → `OnPostAsync()` |
| `GET /Identity/Account/Logout` | `Areas/Identity/Pages/Account/Logout.cshtml.cs` → `OnGet()` |
| `POST /Identity/Account/Logout` | `Areas/Identity/Pages/Account/Logout.cshtml.cs` → `OnPost()` |

---

## 🎯 API ENDPOINT'LERİNİN KONUMLARI

### ⭐ JSON Döndüren API'ler:

1. **`GET /Home/GetNotifications`**
   - **Dosya:** `Controllers/HomeController.cs`
   - **Satır:** 60
   - **Metod:** `GetNotifications()`
   - **Response:** `{ success, notifications[], unreadCount }`

2. **`POST /Home/MarkNotificationAsRead`**
   - **Dosya:** `Controllers/HomeController.cs`
   - **Satır:** 90
   - **Metod:** `MarkNotificationAsRead([FromBody] int id)`
   - **Response:** `{ success }`

3. **`POST /Home/MarkAllNotificationsAsRead`**
   - **Dosya:** `Controllers/HomeController.cs`
   - **Satır:** 106
   - **Metod:** `MarkAllNotificationsAsRead()`
   - **Response:** `{ success }`

4. **`GET /Doctor/GetAvailableDates`**
   - **Dosya:** `Controllers/DoctorController.cs`
   - **Satır:** 336
   - **Metod:** `GetAvailableDates(int? doctorId)`
   - **Response:** `["2025-01-20", "2025-01-21", ...]`

5. **`GET /Doctor/GetAvailableTimeSlots`**
   - **Dosya:** `Controllers/DoctorController.cs`
   - **Satır:** 349
   - **Metod:** `GetAvailableTimeSlots(int? doctorId, string date)`
   - **Response:** `{ success, slots[] }`

---

## 📂 DOSYA YAPISI ÖZET

```
HastaneRandevuSistemi/
├── Controllers/                    ← TÜM ENDPOINT'LER BURADA
│   ├── HomeController.cs           → 7 endpoint
│   ├── DoctorController.cs         → 20 endpoint (2 API)
│   ├── AppointmentController.cs    → 13 endpoint
│   ├── PatientController.cs        → 9 endpoint
│   ├── SpecializationController.cs  → 8 endpoint
│   ├── WorkingHourController.cs     → 8 endpoint
│   ├── AvailabilityBlockController.cs → 8 endpoint
│   └── TestController.cs           → 4 endpoint
│
└── Areas/
    └── Identity/
        └── Pages/
            └── Account/            ← IDENTITY ENDPOINT'LERİ
                ├── Login.cshtml.cs
                ├── Register.cshtml.cs
                └── Logout.cshtml.cs
```

---

## 🔍 ENDPOINT BULMA İPUÇLARI

### 1. Controller Dosyasında Arama:
- Visual Studio'da `Ctrl+F` ile metod adını ara
- Örnek: `GetAvailableDates` → `DoctorController.cs` dosyasında bulunur

### 2. Route Attribute ile Arama:
- Bazı endpoint'ler `[Route]` attribute ile tanımlı olabilir
- `[HttpGet]`, `[HttpPost]` attribute'larına bak

### 3. Metod İmzası ile Arama:
- `public async Task<IActionResult>` ile başlayan metodlar endpoint'dir
- `return Json(...)` döndüren metodlar API endpoint'idir

---

## 💡 ÖRNEK: API ENDPOINT BULMA

**Örnek:** `GetAvailableDates` endpoint'ini bulmak için:

1. **Dosya:** `Controllers/DoctorController.cs`
2. **Satır:** 336
3. **Kod:**
```csharp
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
```

---

**NOT:** Tüm endpoint'ler Controller dosyalarında `public async Task<IActionResult>` metodları olarak tanımlıdır! 🎯

