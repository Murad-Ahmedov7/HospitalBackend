
using Hospital.Business.DTOs.Auth.ResponseDTOs;
using Hospital.Business.Services.Auth.Abstract;
using Hospital.Business.Services.Auth.Concrete;
using Hospital.DataAccess;
using Hospital.DataAccess.Repositories.Auth.Abstract;
using Hospital.DataAccess.Repositories.Auth.Concrete;
using Hospital.Entities.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<HospitalDbContext>(options =>
{
    options.UseSqlServer(conn);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);



builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidateAudience = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            //  Ən vacib dəyişiklik
            NameClaimType = JwtRegisteredClaimNames.Sub // "sub" claim-i Name olaraq qəbul et
            //RoleClaimType = ClaimTypes.Role               // role varsa map et

        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();





//Web API template-i UseAuthorization()-ı hazır qoyur, amma authentication və real qoruma sən yazana qədər mövcud deyil.



//1️⃣ app.UseAuthentication() — SƏN KİMSƏN?
//Nə edir?

//Request-ə baxır

//Authorization: Bearer<token> header-i oxuyur

//Token varsa:

//doğrudurmu ? (signature, exp, issuer, audience)

//kimdir bu user? (claims)

//Əgər hər şey düzdirsə:

//HttpContext.User doldurulur

//Əgər token:

//yoxdursa → user anonymous

//səhvdirsə → auth fail

//❗ Bu mərhələdə icazə yoxlanmır, yalnız kimlik.

//2️⃣ app.UseAuthorization() — BU İSTƏYİ EDƏ BİLƏRSƏN?
//Nə edir?

//HttpContext.User-a baxır

//Endpoint üzərindəki qaydalara baxır:

//[Authorize]

//[Authorize(Roles = "Admin")]

//policy - lər

//Sonra qərar verir:

//bu user buraya girə bilər

//yoxsa 403 / 401 qaytarılmalıdır

//❗ Burada artıq business qaydası işə düşür.

//3️⃣ ƏN VACİB FƏRQ (BUNU QAÇIRMA)
//	Authentication	Authorization
//Sual	Sən kimsən?	Buna icazən var?
//Token oxuyur	✅	❌
//Claims doldurur	✅	❌
//Access qərarı	❌	✅
//4️⃣ NİYƏ SIRA BU QƏDƏR VACİBDİR?
//app.UseAuthentication();
//app.UseAuthorization();


//Əgər tərsinə yazsan:

//app.UseAuthorization();
//app.UseAuthentication();


//❌ Authorization deyəcək:

//“User yoxdur, kimdir bu?”

//və hər şey fail olacaq.

//5️⃣ REAL SƏNARİ
//[Authorize]
//[HttpGet("profile")]
//public IActionResult Profile()


//Flow:

//Request gəlir

//Authentication → token oxunur, user tanınır

//Authorization → [Authorize] icra olunur

//OK → controller işləyir

//6️⃣ ƏN ÇOX EDİLƏN SƏHVLƏR

//❌ “Authorization token-i yoxlayır”
//❌ “Authentication icazə verir”
//❌ “Biri olmasa o biri işləyər”

//Xeyr. Rol bölgüsü dəqiqdir.

//7️⃣ 1 CÜMLƏLİK YEKUN

//Authentication user-i tanıyır, Authorization isə ona icazə verib-vermədiyini yoxlayır.







//1️⃣ KONTEKST – BU FAYL NƏDİR?

//Bu ASP.NET Core Web API-nin startup pipeline-ıdır.

//Burada 3 əsas şey qurulur:

//Servislər (DI)

//Authentication (JWT)

//Middleware sırası

//2️⃣ JWT KONFİQURASİYASI HARADAN BAŞLAYIR?
//🔹 JWT config oxunur
//var jwt = builder.Configuration.GetSection("Jwt");


//Bu, appsettings.json-dakı bu hissəni oxuyur:

//"Jwt": {
//  "Key": "...",
//  "Issuer": "...",
//  "Audience": "...",
//  "ExpireMinutes": 60
//}


//➡️ Hələ heç nə etmir, sadəcə config referansı alır.

//🔹 Key kripto formaya salınır
//var key = Encoding.UTF8.GetBytes(jwt["Key"]);


//Niyə?

//JWT HMAC string yox, byte[] ilə işləyir

//Bu imza yoxlaması üçündür

//➡️ Bu addım mütləqdir və düzgündür

//3️⃣ AUTHENTICATION QURULUR (ƏSAS HİSSƏ)
//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })

//Nə edir?

//Deyir ki:

//“Auth lazımdırsa → JWT Bearer istifadə et”

//Bu o deməkdir ki:

//[Authorize] görəndə

//Authorization: Bearer xxx header-i axtarılacaq

//4️⃣ JWT BEARER NECƏ YOXLANIR?
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters


//Burada JWT-nin qanunları yazılır.

//✅ 1. İMZA YOXLAMASI
//ValidateIssuerSigningKey = true,
//IssuerSigningKey = new SymmetricSecurityKey(key),


//➡️ Token sənin key-inlə imzalanıbmı?
//❌ Deyilsə → reject

//Bu ən vacib təhlükəsizlik nöqtəsidir.

//✅ 2. ISSUER YOXLAMASI
//ValidateIssuer = true,
//ValidIssuer = jwt["Issuer"],


//➡️ Token-i sən yaratmısanmı?

//Başqa serverin token-i → ❌

//✅ 3. AUDIENCE YOXLAMASI
//ValidateAudience = true,
//ValidAudience = jwt["Audience"],


//➡️ Bu token bu API üçünmü buraxılıb?

//Admin token → User API ❌

//✅ 4. VAXT YOXLAMASI
//ValidateLifetime = true,
//ClockSkew = TimeSpan.Zero


//➡️ Token:

//vaxtı bitibsə ❌

//1 saniyə də güzəşt YOXDUR

//ClockSkew = 0 → təhlükəsizlik yönümlü qərar, düzgündür.

//5️⃣ MIDDLEWARE SIRASI (ÇOX KRİTİK)
//app.UseAuthentication();
//app.UseAuthorization();

//Axın belədir:

//Authentication

//token oxunur

//yoxlanır

//HttpContext.User doldurulur

//Authorization

//[Authorize] qaydaları yoxlanır

//➡️ Sıra düzgündür
//Tərsi olsaydı → hər şey qırılardı.

//6️⃣ BU HALDA SİSTEM NECƏ DAVRANIR?

//[Authorize] olmayan endpoint → açıqdır

//[Authorize] olan endpoint:

//token YOX → 401

//token səhv → 401

//token düz, amma icazə yox → 403

//token düz → işləyir ✅

//7️⃣ BU KONFİQURASİYA ÜÇÜN HÖKM

//✔️ Layer baxımından düzgün yerdədir (Hospital.Api)
//✔️ Security baxımından düzgündür
//✔️ Production-a hazır səviyyədədir (key environment-a çıxarılsa)

//8️⃣ 1 CÜMLƏLİK YEKUN

//Bu Program.cs JWT-ni düzgün şəkildə oxuyur, imza–issuer–audience–vaxt yoxlamalarını edir və middleware ardıcıllığını doğru qurur.1️⃣ KONTEKST – BU FAYL NƏDİR?

//Bu ASP.NET Core Web API-nin startup pipeline-ıdır.

//Burada 3 əsas şey qurulur:

//Servislər (DI)

//Authentication (JWT)

//Middleware sırası

//2️⃣ JWT KONFİQURASİYASI HARADAN BAŞLAYIR?
//🔹 JWT config oxunur
//var jwt = builder.Configuration.GetSection("Jwt");


//Bu, appsettings.json-dakı bu hissəni oxuyur:

//"Jwt": {
//  "Key": "...",
//  "Issuer": "...",
//  "Audience": "...",
//  "ExpireMinutes": 60
//}


//➡️ Hələ heç nə etmir, sadəcə config referansı alır.

//🔹 Key kripto formaya salınır
//var key = Encoding.UTF8.GetBytes(jwt["Key"]);


//Niyə?

//JWT HMAC string yox, byte[] ilə işləyir

//Bu imza yoxlaması üçündür

//➡️ Bu addım mütləqdir və düzgündür

//3️⃣ AUTHENTICATION QURULUR (ƏSAS HİSSƏ)
//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    })

//Nə edir?

//Deyir ki:

//“Auth lazımdırsa → JWT Bearer istifadə et”

//Bu o deməkdir ki:

//[Authorize] görəndə

//Authorization: Bearer xxx header-i axtarılacaq

//4️⃣ JWT BEARER NECƏ YOXLANIR?
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters


//Burada JWT-nin qanunları yazılır.

//✅ 1. İMZA YOXLAMASI
//ValidateIssuerSigningKey = true,
//IssuerSigningKey = new SymmetricSecurityKey(key),


//➡️ Token sənin key-inlə imzalanıbmı?
//❌ Deyilsə → reject

//Bu ən vacib təhlükəsizlik nöqtəsidir.

//✅ 2. ISSUER YOXLAMASI
//ValidateIssuer = true,
//ValidIssuer = jwt["Issuer"],


//➡️ Token-i sən yaratmısanmı?

//Başqa serverin token-i → ❌

//✅ 3. AUDIENCE YOXLAMASI
//ValidateAudience = true,
//ValidAudience = jwt["Audience"],


//➡️ Bu token bu API üçünmü buraxılıb?

//Admin token → User API ❌

//✅ 4. VAXT YOXLAMASI
//ValidateLifetime = true,
//ClockSkew = TimeSpan.Zero


//➡️ Token:

//vaxtı bitibsə ❌

//1 saniyə də güzəşt YOXDUR

//ClockSkew = 0 → təhlükəsizlik yönümlü qərar, düzgündür.

//5️⃣ MIDDLEWARE SIRASI (ÇOX KRİTİK)
//app.UseAuthentication();
//app.UseAuthorization();

//Axın belədir:

//Authentication

//token oxunur

//yoxlanır

//HttpContext.User doldurulur

//Authorization

//[Authorize] qaydaları yoxlanır

//➡️ Sıra düzgündür
//Tərsi olsaydı → hər şey qırılardı.

//6️⃣ BU HALDA SİSTEM NECƏ DAVRANIR?

//[Authorize] olmayan endpoint → açıqdır

//[Authorize] olan endpoint:

//token YOX → 401

//token səhv → 401

//token düz, amma icazə yox → 403

//token düz → işləyir ✅

//7️⃣ BU KONFİQURASİYA ÜÇÜN HÖKM

//✔️ Layer baxımından düzgün yerdədir (Hospital.Api)
//✔️ Security baxımından düzgündür
//✔️ Production-a hazır səviyyədədir (key environment-a çıxarılsa)

//8️⃣ 1 CÜMLƏLİK YEKUN

//Bu Program.cs JWT-ni düzgün şəkildə oxuyur, imza–issuer–audience–vaxt yoxlamalarını edir və middleware ardıcıllığını doğru qurur.







//1️⃣ Problem: Login / JWT istifadə edərkən nə baş verirdi?

//Sənin backend-də hal-hazırda vəziyyət belə idi:

//User login olur → token yaradılır:

//private string GenerateJwtToken(User user)
//{
//    var claims = new List<Claim>
//    {
//        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
//        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
//    };
//    ...
//}


//Token payload-u belə görünürdü:

//{
//    "sub": "5",
//  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "5",
//  "exp": 1700003600
//}


//Backend - də user - i oxumaq üçün istifadə edilirdi:

//var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


//Problemlər:

//Problem Niyə?
//nameidentifier claim-i token-də idi	Duplication → həm sub, həm nameidentifier eyni məlumat
//User.Identity.Name null	Default mapping yoxdur, Name claim token-də yox idi
//Real pro-level token?	Token artıq və spesifik ASP.NET-ə bağlıdır → microservices, mobil apps üçün universal deyil
//2️⃣ Həll yolu: Pro - style JWT
//2.1 Token-u sadələşdirdik

//Yalnız sub claim-i saxladıq

//Optional: exp, role, permissions

//{
//  "sub": "5",
//  "exp": 1700003600
//}


//Artıq:

//Token universal oldu(JWT spec - ə uyğun)

//Duplication aradan qalxdı

//nameidentifier lazım deyil

//2.2 ASP.NET Core mapping əlavə etdik
//services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//                                            {
//                                                ValidateIssuerSigningKey = true,
//                                                IssuerSigningKey = new SymmetricSecurityKey(key),

//                                                ValidateIssuer = true,
//                                                ValidateAudience = true,
//                                                ValidIssuer = jwt["Issuer"],
//                                                ValidAudience = jwt["Audience"],

//                                                ValidateLifetime = true,
//                                                ClockSkew = TimeSpan.Zero,

//                                                // 🔥 Əsas dəyişiklik:
//                                                NameClaimType = JwtRegisteredClaimNames.Sub,   // sub → Name/NameIdentifier
//                                                RoleClaimType = ClaimTypes.Role
//                                            };
//    });


//Nəticə:

//User.Identity.Name → sub claim-i

//User.FindFirst(ClaimTypes.NameIdentifier) → sub claim-i

//Token-də artıq nameidentifier yoxdur

//Kod daha clean və universal oldu

//3️⃣ Backend-də istifadə

//Əvvəlki kimi artıq problem yoxdur:

//// userId almaq
//var userId = User.Identity?.Name; // "5"

//// və ya
//var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // "5"


//Əlavə rahatlıq:

//Hər endpoint(ChangePassword, Profile, Orders) üçün token kifayətdir

//Frontend token-i saxlayır, userId ayrı göndərməyə ehtiyac yoxdur

//4️⃣ Nəticə: Nə dəyişdi?
//Əvvəl	İndi
//sub + nameidentifier → duplication	Yalnız sub → clean
//User.Identity.Name null	User.Identity.Name → sub claim
//Token ASP.NET-only	Token universal, pro-level
//Backend userId oxumaq qarışıq	Backend rahat: User.Identity.Name / FindFirstValue(JwtRegisteredClaimNames.Sub)

//💡 Qısaca:

//Problem = sub token - də var, amma backend default NameIdentifier gözləyir → duplication və qarışıqlıq

//Həll = token-də yalnız sub, ASP.NET Core-da NameClaimType = JwtRegisteredClaimNames.Sub map

//Nəticə = clean, pro-level, universal JWT, bütün endpoints rahat işləyir

//Əgər istəyirsənsə, mən sənə tam real JWT generator + ASP.NET Core setup + ChangePassword endpoint nümunəsini industry standard olaraq yaza bilərəm,
//beləliklə tam professional proyekt setup olacaq.

//Bunu yazım?



//Bəli 😎 tam olaraq belədir. Gəlin bunu açıq şəkildə başa düşək:

//Token payload(sadə nümunə)
//{
//    "sub": "5",
//  "exp": 1700003600
//}


//sub = 5 → user - id(JWT standard claim)

//Backend - də mapping sayəsində
//options.TokenValidationParameters = new TokenValidationParameters
//                                    {
//                                        NameClaimType = JwtRegisteredClaimNames.Sub,
//                                        RoleClaimType = ClaimTypes.Role
//                                    };


//Bu mapping ilə ASP.NET Core deyir:

//“Hər yerdə Name / NameIdentifier lazım olsa, token-dəki sub-u istifadə et”

//Nəticədə:
//Backend call	Nəticə
//User.Identity.Name	"5" → token-dəki sub claim-i
//User.FindFirst(ClaimTypes.NameIdentifier)?.Value	"5" → token-dəki sub claim-i

//✅ Hər ikisi token-dəki user-id-dir.

//Yəni praktiki olaraq:

//sub token-də bir dəfə var

//nameidentifier claim-inə ehtiyac yoxdur

//User.Identity.Name və User.FindFirst(ClaimTypes.NameIdentifier) → hər ikisi həmin user-id-i qaytarır

//İstəsən, mən bunu gözlə görünən diagramla göstərim, beləliklə hər şeyi vizual görə bilərsən və qarışıq qalmayacaq.










//bele baxanda onsuz ikiside tam eyni isi gorur de ne ferqi var

//Tam olaraq belədir 😄 — səni çaşdıran da elə budur: “ikisi eyni şeyi qaytarır”. Amma detal fərqləri və practical kontekst var. Gəlin bunu tam açıq izah edim:

//1️⃣ Eyni işi görürlər

//Sənin pro-level setup-da (NameClaimType = JwtRegisteredClaimNames.Sub)

//User.Identity.Name → "5"

//User.FindFirst(ClaimTypes.NameIdentifier)?.Value → "5"

//💥 Hər ikisi token-dəki sub claim-i qaytarır, nəticə tam eynidir.

//2️⃣ Fərq hardadır?
//Aspect	User.Identity.Name	User.FindFirst(ClaimTypes.NameIdentifier)
//Sadəlik	Daha qısa, oxumaq rahat: User.Identity.Name Daha uzun, FindFirst çağırmaq lazımdır
//Mapping dependency	Əgər NameClaimType = sub set olunubsa işləyir	Mapping tələb etmir, claim tipi birbaşa göstərilir
//Use case	Sadə user-id, logging, basic endpoints	Middleware, policies, role-based checks, bir neçə claim-lə işləyən kompleks senarilər
//Readability	Clean və “pro-style”	Bir az daha verbose, amma universal claim access verir
//3️⃣ Practical nümunə

//Sadə endpoint:

//[Authorize]
//[HttpPost("change-password")]
//public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
//{
//    var userId = User.Identity.Name; // daha sadə və clean
//    ...
//}


//Advanced / middleware / policy:

//var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//var role = User.FindFirst(ClaimTypes.Role)?.Value;

//if (role != "Admin")
//    return Forbid();


//Burda sən bir neçə claim-lə işləyirsən, FindFirst daha rahatdır

//4️⃣ Qısaca

//Token mapping varsa → User.Identity.Name = FindFirst(NameIdentifier)

//Mapping yoxdursa → FindFirst lazımdır

//Pro-lar adətən User.Identity.Name istifadə edir, çünki qısa, clean, oxunaqlı və standart

//Əgər istəyirsənsə, mən sənə tam real ChangePassword endpoint nümunəsini həm User.Identity.Name, həm FindFirst ilə göstərə bilərəm, beləliklə hansı vəziyyətdə hansını istifadə edəcəyini vizual görəcəksən.

//Bunu göstərim?





//biz evvel mapping etmyende yeni birbasa tokenin icinde yazanda da  User.Identity.Name null bele de yaza bilmirdik duzdu?

//Bəli, tam düzgün başa düşdün ✅

//Gəlin bunu addım-addım izah edim ki, qarışmasın:

//1️⃣ Mapping olmadan vəziyyət

//Token yaratdın:

//new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
//// nameidentifier claim yoxdur


//ASP.NET Core default olaraq User.Identity.Name üçün ClaimTypes.Name-ə baxır

//➡ Sənin token-də ClaimTypes.Name yoxdur, yalnız sub var

//Nəticə:

//var userId = User.Identity.Name; // null


//Çünki NameClaimType default = ClaimTypes.Name

//Token-də isə Name claim yoxdur → null dönür

//2️⃣ Mapping olmadan “bircə token içində yazmaq” kifayət etmirdi

//Yəni token-də yalnız sub olsa da, User.Identity.Name istifadə etmək olmurdu

//Alternativ olaraq hər dəfə FindFirst("sub") yazmalı idin:

//var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value; // işləyirdi


//Bir az verbose və repetitiv idi

//User.Identity.Name → null olduğu üçün birbaşa istifadə etmək mümkün deyildi

//3️⃣ Mapping etdikdən sonra
//NameClaimType = JwtRegisteredClaimNames.Sub


//ASP.NET Core deyir:

//“Name / NameIdentifier lazım olduqda token-dəki sub-u istifadə et”

//İndi:

//var userId = User.Identity.Name; // artıq "5" qaytarır


//Daha clean və universal oldu ✅

//Qısaca
//Vəziyyət	User.Identity.Name	Qeyd
//Mapping olmadan, token-də yalnız sub	null	FindFirst istifadə etmək lazımdır
//Mapping ilə (NameClaimType = sub)	"5"	Sadəcə User.Identity.Name kifayət edir

//Əgər istəyirsənsə, mən sənə tam real nümunə – login, token generation, mapping və ChangePassword endpoint ilə göstərə bilərəm,
//beləliklə bütün flow-u birdəfəlik vizual görə bilərsən.

//Bunu edim?



//net core umumiyetle mapping anlayisi nedi






//3️⃣ Login response

//Minimal və professional: yalnız token-i göndər

//return Ok(new LoginResponseDto
//          {
//              Token = token!
//          });


//Frontend üçün optional:

//Frontend lazım olduqda token-i decode edib userId və expire time ala bilər

//const decoded = jwt_decode(token);
//const userId = decoded.sub;              // token-dəki user id
//const expiresAt = decoded.exp * 1000;    // JS timestamp


//Əgər gələcəkdə lazım olarsa, login response-a UserId və ExpiresIn əlavə etmək olar:

//return Ok(new LoginResponseDto
//{
//    Token = token!,
//    UserId = user.Id,
//    ExpiresIn = (int)(expireTime - DateTime.UtcNow).TotalSeconds
//});