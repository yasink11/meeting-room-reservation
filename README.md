# Toplantı Odası Rezervasyon Sistemi

.NET Core 6.0 ile geliştirilmiş toplantı odası rezervasyon yönetim API'si.

## 🎯 Proje Özeti

Şirket içi toplantı odalarının rezervasyonunu yöneten RESTful Web API. Çakışan rezervasyonları engelleyen, kapasite kontrolü yapan ve tekrarlayan toplantıları destekler.

**Geliştirme Süresi:** ~6-7 saat

## 🛠️ Teknolojiler

- .NET Core 6.0
- Entity Framework Core 6.0.25
- SQL Server
- FluentValidation 11.3.0
- Swagger/OpenAPI

## 🚀 Kurulum

### Gereksinimler
- .NET 6.0 SDK
- SQL Server (LocalDB/Express)
- Visual Studio 2019+

### Adımlar

1. Projeyi klonlayın:
```bash
git clone https://github.com/yasink11/meeting-room-reservation.git
cd meeting-room-reservation
```

2. `appsettings.json` dosyasında connection string'i güncelleyin:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=MeetingRoomDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

3. Veritabanını oluşturun:
```bash
dotnet ef database update
```

4. Çalıştırın:
```bash
dotnet run
```

Swagger UI: `https://localhost:7xxx/swagger`

---

## 📜 İş Kuralları Tasarımı

Case study'de iş kurallarını kendimiz tasarlamamız istendi.

### 1. Çakışan Rezervasyonlar
❌ **Kural:** Aynı oda, aynı saatte birden fazla rezervasyon YAPILAMAZ.

**Gerekçe:** Fiziksel kısıt - bir oda aynı anda iki toplantıya ev sahipliği yapamaz.

**Kontrol:** `(Yeni.Başlangıç < Mevcut.Bitiş) AND (Yeni.Bitiş > Mevcut.Başlangıç)`

### 2. Rezervasyon Süreleri
- ⏱️ **Minimum:** 15 dakika (çok kısa toplantılar verimsiz)
- ⏱️ **Maksimum:** 8 saat (tüm gün rezervasyonu diğerlerini engeller)
- 📅 **Geçmiş tarih:** İZİN YOK
- 📅 **Maksimum gelecek:** 3 ay (belirsizlik azaltmak için)

### 3. İptal Politikası
⏰ **Kural:** Toplantıdan minimum 1 saat önce iptal edilebilir.

**Gerekçe:** Son dakika iptalleri diğerlerinin odayı bulmasını engeller. 1 saat makul bir pencere.

### 4. Kapasite Kontrolü
👥 **Kural:** Katılımcı sayısı oda kapasitesini AŞAMAZ (hard limit).

**Gerekçe:** Güvenlik (yangın yönetmelikleri) ve konfor.

**Davranış:** Hata döner, geçiş yok.

### 5. Kullanıcı Kısıtlamaları
👤 **Kural:** Bir kullanıcı aynı anda farklı odalarda bile çakışan rezervasyon yapamaz.

**Gerekçe:** Fiziksel kısıt - bir kişi iki yerde olamaz.

---

## 🔄 Tekrarlayan Toplantılar Çözümü

### Problem
Case study özel zorluğu: "Her Pazartesi 10:00-11:00, 8 hafta, 3. hafta tatil"

### Seçtiğim Yaklaşım: Ana Rezervasyon + Exception Kayıtları

**Veritabanı:**
- `RecurringGroup` tablosu: Tekrarlama kuralları (pattern, interval, exceptionDates)
- `Reservation` tablosu: Her toplantı ayrı kayıt + RecurringGroupId
- Exception dates: Virgülle ayrılmış string ("2025-03-10,2025-04-15")

**Neden Bu Yaklaşım?**

✅ Avantajlar:
- Her rezervasyon bağımsız değiştirilebilir
- Tek tekrarı iptal edebilme
- Hızlı sorgulama (join'siz)
- Basit çakışma kontrolü

❌ Dezavantajlar:
- Çok kayıt (52 hafta = 52 kayıt)
- Toplu güncelleme biraz zor

**Alternatifler:**
1. **Cron Pattern:** Az yer ama karmaşık sorgular
2. **Event Sourcing:** Tam audit ama aşırı karmaşık

**Seçim gerekçesi:** Bu proje ölçeği için basitlik ve performans dengesi en iyi.

---

## 🗄️ Veritabanı Şeması

### Rooms
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | PK |
| Name | nvarchar(100) | Indexed |
| Capacity | int | Max kişi |
| Floor | int | Kat |
| Equipment | nvarchar(500) | Virgülle ayrılmış |
| IsActive | bit | Soft delete |

### Reservations
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | PK |
| RoomId | int | FK → Rooms |
| RecurringGroupId | int? | FK → RecurringGroups |
| UserName | nvarchar(100) | |
| Title | nvarchar(200) | |
| StartTime | datetime2 | Indexed |
| EndTime | datetime2 | Indexed |
| IsCancelled | bit | Soft delete |

**Index:** `(RoomId, StartTime, EndTime)` - Çakışma kontrolü için

### RecurringGroups
| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | int | PK |
| Pattern | nvarchar(50) | Weekly/Daily/Monthly |
| Interval | int | 1=her hafta |
| DayOfWeek | nvarchar(20) | Monday, Tuesday... |
| ExceptionDates | nvarchar(2000) | "2025-03-10,..." |

**Seed Data:** 3 örnek oda otomatik eklenir.

---

## 🌐 API Endpoints

### Rooms
- `GET /api/rooms` - Tüm odalar
- `GET /api/rooms/{id}` - Tekil oda
- `POST /api/rooms` - Oda oluştur
- `PUT /api/rooms/{id}` - Oda güncelle
- `DELETE /api/rooms/{id}` - Oda sil (soft)

### Reservations
- `GET /api/reservations` - Tüm rezervasyonlar
- `GET /api/reservations/{id}` - Tekil rezervasyon
- `POST /api/reservations` - Rezervasyon oluştur
- `POST /api/reservations/recurring` - Tekrarlayan rezervasyon
- `PUT /api/reservations/{id}` - Rezervasyon güncelle
- `DELETE /api/reservations/{id}` - İptal et (soft)
- `GET /api/reservations/conflicts?roomId=1&start=...&end=...` - Çakışmaları listele

**Response Format:**
```json
{
  "success": true,
  "data": { },
  "message": "İşlem başarılı",
  "errors": []
}
```

---

## 🔒 Güvenlik

- **SQL Injection:** EF Core parameterized queries (otomatik)
- **XSS:** ASP.NET Core otomatik encoding
- **Input Validation:** FluentValidation
- **Exception Handling:** Global middleware
- **HTTPS:** Development'ta otomatik

**Uygulanmadı (basitlik için):**
- Authentication/Authorization
- Rate limiting

---

## 🏗️ Mimari
```
Controllers → Services → DbContext → Database
     ↓           ↓
   DTOs     Validators
```

- **Service Layer Pattern:** İş mantığı service'lerde
- **DTO Pattern:** Entity'ler ≠ API response
- **Dependency Injection:** Tüm service'ler DI ile
- **Soft Delete:** IsActive, IsCancelled flag'leri

---

## 📝 Varsayımlar

1. **Auth:** Kullanıcı adı string (JWT yok)
2. **Equipment:** Virgülle ayrılmış string (normalizasyon yok)
3. **Timezone:** Local time (UTC değil)
4. **Pagination:** Yok (küçük dataset varsayımı)

---

## 🎯 Değerlendirme Kriterleri Karşılama

| Kriter | Uygulama |
|--------|----------|
| **Veritabanı Tasarımı** | ✅ Normalize, indexler, foreign keys, seed data |
| **Clean Code** | ✅ SOLID, service layer, anlamlı isimler |
| **İş Kuralları** | ✅ 5 kural + dokümantasyon |
| **Exception Handling** | ✅ Global middleware |
| **Validation** | ✅ FluentValidation + iş kuralları |
| **Tekrarlayan Toplantılar** | ✅ Exception dates ile |
| **API Tasarımı** | ✅ RESTful, standart response |

---

---

### 📦 Postman

- `MeetingRoomReservation.postman_collection.json`
- `MeetingRoomReservation.postman_environment.json`

### 🔹 Kullanım

1. Postman’i açın
2. **Import** → Collection dosyasını seçin
3. Tekrar **Import** → Environment dosyasını seçin
4. Sağ üstten environment olarak `MeetingRoomReservation` seçin
5. API’yi çalıştırmadan önce uygulamanın ayakta olduğundan emin olun

Base URL varsayılan olarak: https://localhost:7195/

---


## 👤 Geliştirici

Yasin Karaçam

**GitHub:** [https://github.com/yasink11/meeting-room-reservation](https://github.com/yasink11/meeting-room-reservation)

**Geliştirme Tarihi:18 Şubat 2025
