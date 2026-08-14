# JwtMusic — Case 10

JWT ve paket bazlı yetkilendirme kullanan iki katmanlı müzik platformu. API ve MVC Web UI ayrı projelerdir. Veritabanı ilk çalıştırmada `JwtMusic.WebApi/JwtMusic.db` olarak otomatik oluşur; 15 yerli ve 15 yabancı popüler parçanın resmi 30 saniyelik Apple Music önizlemesi, albüm kapağı, sanatçı, albüm ve tür bilgileri kataloğa eklenir. İlk katalog kurulumu için internet bağlantısı gerekir.

## Çalıştırma

İki terminal açın:

```powershell
dotnet run --project JwtMusic.WebApi
dotnet run --project JwtMusic.WebUI
```

- Web UI: `http://localhost:5220`
- Swagger: `http://localhost:5155/swagger`

Visual Studio kullanıyorsanız iki projeyi birlikte başlangıç projesi olarak seçebilirsiniz.

## Demo hesaplar

| Kullanıcı | Parola | Paket |
|---|---|---|
| `basic` | `Music123` | Basic |
| `gold` | `Music123` | Gold |
| `premium` | `Music123` | Premium |
| `elit` | `Music123` | Elit |

Yeni kayıtlar rol ataması yapılmadan Basic paketle oluşturulur. Login yanıtındaki JWT, `package` claim'ini içerir. Üst paket kullanıcıları alt paket şarkılarını dinleyebilir; yetersiz paketle stream isteği `403 Forbidden` döndürür.

## Temel API akışı

1. `POST /api/Register` ile kullanıcı oluşturun.
2. `POST /api/Login` ile JWT alın.
3. Swagger veya Postman'de `Authorization: Bearer <token>` başlığını ekleyin.
4. `GET /api/songs` ile kataloğu alın.
5. `GET /api/songs/{id}/stream` ile paket kontrolünden geçen resmi ses önizlemesini oynatın.

Diğer uçlar: `/api/artists`, `/api/genres`, `/api/albums`, `/api/playlists`, `/api/users/me`, `/api/users/me/history`.
