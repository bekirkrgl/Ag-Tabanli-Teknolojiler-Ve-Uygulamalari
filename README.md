# 🏥 Hastane Randevu Sistemi

Modern ve kullanıcı dostu bir hastane randevu yönetim sistemi. ASP.NET Core 8.0, Entity Framework Core ve Identity kullanılarak geliştirilmiştir.

## 📋 Özellikler

### 🩺 Doktor Paneli
- Randevularını görüntüleme ve yönetme
- Müsaitlik saatleri ayarlama
- Randevu durumlarını güncelleme (Onayla/İptal/Tamamla)
- Hasta bilgilerini görüntüleme
- Özgeçmiş ve çalışma saatleri yönetimi

### 👤 Hasta Paneli
- Doktor ve branş seçerek randevu alma
- Mevcut randevularını görüntüleme
- Randevu geçmişi
- Doktor bilgilerini görme
- Şikayet ve notları ekleme

### 🔐 Güvenlik ve Yetkilendirme
- Kullanıcı giriş/çıkış sistemi
- Rol tabanlı yetkilendirme (Doctor, Patient, Admin)
- Güvenli şifre politikası
- Identity entegrasyonu

### 🔔 Bildirim Sistemi
- Randevu hatırlatmaları
- Durum değişikliği bildirimleri
- Anlık bildirimler

### 📊 Dashboard
- Sistem istatistikleri
- Aktif doktor ve hasta sayıları
- Toplam randevu sayısı

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- **.NET 8.0 SDK** ([İndir](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server LocalDB** (Windows'ta genellikle yüklü gelir)
- **Visual Studio Code** veya **Visual Studio** (isteğe bağlı)

### Kurulum Adımları

#### 1. Projeyi İndir/Klon
```bash
# Proje klasörünü açın
cd "proje-klasörü"
```

#### 2. Paketleri Yükle
```bash
dotnet restore
```

#### 3. Veritabanını Oluştur
```bash
dotnet ef database update
```

#### 4. Projeyi Çalıştır
```bash
dotnet run
```

#### 5. Tarayıcıda Aç
- **HTTP:** http://localhost:5090
- **HTTPS:** https://localhost:7015

## 👥 Test Kullanıcıları

Sistem otomatik olarak aşağıdaki test kullanıcılarını oluşturur:

### Doktor
- **Email:** doktor@test.com
- **Şifre:** Doktor123!

### Hasta
- **Email:** hasta@test.com
- **Şifre:** Hasta123!

## 🎯 Kullanım

### İlk Giriş
1. Ana sayfada "Giriş Yap" butonuna tıklayın
2. Test kullanıcı bilgilerinden birini girin
3. İlgili paneline yönlendirilirsiniz

### Randevu Alma (Hasta)
1. Ana sayfadan doktor seçin
2. "Randevu Al" butonuna tıklayın
3. Tarih ve saat seçin
4. Şikayet/not ekleyin
5. Randevuyu onaylayın

### Randevu Yönetimi (Doktor)
1. Doktor paneline giriş yapın
2. Randevular sekmesinden mevcut randevuları görün
3. Durumları güncelleyin (Onayla/İptal/Tamamla)
4. Müsaitlik saatlerini ayarlayın

## 📁 Proje Yapısı

```
HastaneRandevuSistemi/
├── Controllers/          # Controller'lar
│   ├── AppointmentController.cs
│   ├── DoctorController.cs
│   ├── PatientController.cs
│   └── ...
├── Models/               # Veri modelleri
│   ├── Appointment.cs
│   ├── Doctor.cs
│   ├── Patient.cs
│   └── ...
├── Views/                # Razor view'lar
│   ├── Doctor/
│   ├── Patient/
│   ├── Appointment/
│   └── ...
├── Data/                 # Veritabanı context
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
├── Services/             # Servisler
│   ├── DoctorAvailabilityService.cs
│   ├── NotificationService.cs
│   └── ...
└── wwwroot/              # Static dosyalar
```

## 🔧 Teknolojiler

- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM
- **SQL Server** - Veritabanı
- **Identity** - Kullanıcı yönetimi
- **Bootstrap 5** - CSS framework
- **Razor Pages** - View engine

## 📝 Veritabanı

Proje **LocalDB** kullanır. İlk çalıştırmada:
- Veritabanı otomatik oluşturulur
- Örnek veriler eklenir
- Seed data ile test edilebilir

### Veritabanı Bağlantısı
```json
Server=(localdb)\mssqllocaldb;
Database=HastaneRandevuSistemiDb;
Trusted_Connection=true;
```

## 🎨 Özellikler ve İstatistikler

- ✅ 12+ Doktor
- ✅ 5+ Örnek Hasta
- ✅ 15+ Branş
- ✅ 40+ Örnek Randevu
- ✅ Tam CRUD işlemleri
- ✅ Role-based authorization
- ✅ Responsive tasarım

## 🐛 Sorun Giderme

### Port Hatası
```bash
# Belirli bir portta çalıştırma
dotnet run --urls "https://localhost:5090"
```

### Veritabanı Hatası
```bash
# Veritabanını sıfırla
dotnet ef database drop
dotnet ef database update
```

### Paket Hatası
```bash
# Temizle ve yeniden yükle
dotnet clean
dotnet restore
```

## 📄 Lisans

Bu proje eğitim amaçlıdır.

## 👨‍💻 Geliştirici

- Proje: Hastane Randevu Sistemi
- Framework: ASP.NET Core 8.0
- Tarih: 2024

---

**Not:** İlk çalıştırmada `dotnet restore` komutunu çalıştırmayı unutmayın!

