
using Hospital.Business.Services.Auth.Abstract;
using Hospital.Business.Services.Auth.Concrete;
using Hospital.DataAccess;
using Hospital.DataAccess.Repositories.Auth.Abstract;
using Hospital.DataAccess.Repositories.Auth.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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
            ClockSkew = TimeSpan.Zero
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