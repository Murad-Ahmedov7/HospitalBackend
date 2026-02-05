using Azure.Core;
using Hospital.Business.DTOs.Auth.RequestDTOs;
using Hospital.Business.Enums;
using Hospital.Business.Services.Auth.Abstract;
using Hospital.DataAccess.Repositories.Auth.Abstract;
using Hospital.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace Hospital.Business.Services.Auth.Concrete
{
    
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<bool> RegisterAsnyc(RegisterRequestDto dto)
        {
            var passwordHash=BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user=new User(dto.FullName,dto.Email,passwordHash,dto.Phone);

           await _userRepository.AddAsync(user);

           await _userRepository.SaveChangesAsync();


            return true;
        }

        public async Task <(LoginStatus status,string? token,Guid? userId,int? expiresIn)> LoginAsnyc(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);


            if (user == null)
            {
                return(LoginStatus.InvalidCredentials,null,null,null);
            }

            var passwordValid =BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);



            //Enum type olsa da, onun üzvləri yalnız type adı ilə istifadə olunur.
            //Enum - dan olan dəyişənlər üzərindən enum üzvlərinə müraciət etmək mümkün deyil.
            //Doğru: LoginStatus.Success
            //Yanlış: status.Success



            //Controller - də status enum-un dəyərini saxlayan dəyişəndir.
            //Enum üzvləri (Success, InvalidCredentials və s.) isə yalnız enum-un tipi(LoginStatus) vasitəsilə istifadə olunur.



            //Server, istifadəçinin yazdığı parolu, DB - dəki hash -in içində olan eyni salt və eyni qayda ilə yenidən hash edir və nəticəni müqayisə edir.

            if (!passwordValid)
            {
                return(LoginStatus.InvalidCredentials,null,null,null);
            }

            var token = GenerateJwtToken(user);
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);

            return (LoginStatus.Success,token,user.Id,expireMinutes*60);



            //Biz property-lərə(result.token və s.) müraciət edə bilirik, çünki funksiya adlandırılmış tuple(named tuple) qaytarır.


            //Tuple - da null yaza bilməyinin səbəbi onun elementlərinin nullable(?) elan olunmasıdır; DTO - da isə Guid və int nullable olmadığı üçün bu mümkün deyil.


        }



        // ================= JWT GENERATION =================   
        private string GenerateJwtToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var keyString = jwtSection["Key"]
                ?? throw new Exception("JWT Key is missing in appsettings.json");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyString)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                // JWT standard
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),

                // ASP.NET Core üçün rahatlıq
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var expireMinutes = int.Parse(jwtSection["ExpireMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

//2️⃣ GenerateJwtToken — JWT NECƏ YARADILIR?

//Bu metodun işi:
//👉 user məlumatını götür → imzala → JWT string qaytar

//🔹 Addım 1 — JWT config oxunur
//var jwtSection = _configuration.GetSection("Jwt");


//Bu hissəni oxuyur:

//"Jwt": {
//  "Key": "...",
//  "Issuer": "...",
//  "Audience": "...",
//  "ExpireMinutes": 60
//}

//🔹 Addım 2 — Secret key hazırlanır
//var keyString = jwtSection["Key"]
//    ?? throw new Exception("JWT Key is missing");


//Key olmazsa → sistem işləməməlidir

//Bu düzgün yanaşmadır

//var key = new SymmetricSecurityKey(
//    Encoding.UTF8.GetBytes(keyString)
//);


//👉 String → byte[]
//👉 HMAC SHA256 yalnız byte[] ilə işləyir

//🔹 Addım 3 — Signing credentials
//var credentials = new SigningCredentials(
//    key,
//    SecurityAlgorithms.HmacSha256
//);


//Bu deməkdir ki:

//“Token bu key ilə imzalanacaq”

//⚠️ Burada encrypt yoxdur, yalnız sign var.

//🔹 Addım 4 — Claims yığılır
//var claims = new List<Claim>
//{
//    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

//sub

//JWT standard claim

//“Bu token kim üçündür?”

//new Claim(
//    JwtRegisteredClaimNames.Iat,
//    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
//    ClaimValueTypes.Integer64),

//iat

//Token nə vaxt yaradılıb

//Unix timestamp

//Security üçün faydalıdır

//new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

//NameIdentifier

//ASP.NET Core üçün rahatlıq

//HttpContext.User.FindFirst(...) ilə asan oxunur

//🔹 Addım 5 — Expire hesablanır
//var expireMinutes =
//    int.Parse(jwtSection["ExpireMinutes"]!);


//Token:

//expires: DateTime.UtcNow.AddMinutes(expireMinutes)


//👉 Bu vaxt keçəndən sonra token ölür

//🔹 Addım 6 — Token yaradılır
//var token = new JwtSecurityToken(
//    issuer: jwtSection["Issuer"],
//    audience: jwtSection["Audience"],
//    claims: claims,
//    expires: ...,
//    signingCredentials: credentials
//);


//Burada:

//kim yaradıb → issuer

//kim üçündür → audience

//içində nə var → claims

//nə vaxta qədər → expires

//kim imzalayıb → key

//🔹 Addım 7 — String-ə çevrilir
//return new JwtSecurityTokenHandler().WriteToken(token);


//➡️ Nəticə:

//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...


//Bu:

//DB-də saxlanmır

//server-də saxlanmır

//client-ə göndərilir

//3️⃣ ÜMUMİ AXIN (BÜTÖV ŞƏKİL)
//Client → Login (email + password)
//        ↓
//AuthService
//  → User yoxlanır
//  → Parol yoxlanır
//  → JWT yaradılır
//        ↓
//Client token alır
//        ↓
//Sonrakı request-lər:
//Authorization: Bearer <token>
//2️⃣ GenerateJwtToken — JWT NECƏ YARADILIR?

//Bu metodun işi:
//👉 user məlumatını götür → imzala → JWT string qaytar

//🔹 Addım 1 — JWT config oxunur
//var jwtSection = _configuration.GetSection("Jwt");


//Bu hissəni oxuyur:

//"Jwt": {
//  "Key": "...",
//  "Issuer": "...",
//  "Audience": "...",
//  "ExpireMinutes": 60
//}

//🔹 Addım 2 — Secret key hazırlanır
//var keyString = jwtSection["Key"]
//    ?? throw new Exception("JWT Key is missing");


//Key olmazsa → sistem işləməməlidir

//Bu düzgün yanaşmadır

//var key = new SymmetricSecurityKey(
//    Encoding.UTF8.GetBytes(keyString)
//);


//👉 String → byte[]
//👉 HMAC SHA256 yalnız byte[] ilə işləyir

//🔹 Addım 3 — Signing credentials
//var credentials = new SigningCredentials(
//    key,
//    SecurityAlgorithms.HmacSha256
//);


//Bu deməkdir ki:

//“Token bu key ilə imzalanacaq”

//⚠️ Burada encrypt yoxdur, yalnız sign var.

//🔹 Addım 4 — Claims yığılır
//var claims = new List<Claim>
//{
//    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

//sub

//JWT standard claim

//“Bu token kim üçündür?”

//new Claim(
//    JwtRegisteredClaimNames.Iat,
//    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
//    ClaimValueTypes.Integer64),

//iat

//Token nə vaxt yaradılıb

//Unix timestamp

//Security üçün faydalıdır

//new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

//NameIdentifier

//ASP.NET Core üçün rahatlıq

//HttpContext.User.FindFirst(...) ilə asan oxunur

//🔹 Addım 5 — Expire hesablanır
//var expireMinutes =
//    int.Parse(jwtSection["ExpireMinutes"]!);


//Token:

//expires: DateTime.UtcNow.AddMinutes(expireMinutes)


//👉 Bu vaxt keçəndən sonra token ölür

//🔹 Addım 6 — Token yaradılır
//var token = new JwtSecurityToken(
//    issuer: jwtSection["Issuer"],
//    audience: jwtSection["Audience"],
//    claims: claims,
//    expires: ...,
//    signingCredentials: credentials
//);


//Burada:

//kim yaradıb → issuer

//kim üçündür → audience

//içində nə var → claims

//nə vaxta qədər → expires

//kim imzalayıb → key

//🔹 Addım 7 — String-ə çevrilir
//return new JwtSecurityTokenHandler().WriteToken(token);


//➡️ Nəticə:

//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...


//Bu:

//DB-də saxlanmır

//server-də saxlanmır

//client-ə göndərilir

//3️⃣ ÜMUMİ AXIN (BÜTÖV ŞƏKİL)
//Client → Login (email + password)
//        ↓
//AuthService
//  → User yoxlanır
//  → Parol yoxlanır
//  → JWT yaradılır
//        ↓
//Client token alır
//        ↓
//Sonrakı request-lər:
//Authorization: Bearer <token>














//SƏNİN GÖRDÜYÜN SƏTİR
//var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);


//Bu sətir sadə görünür, amma 3 şey eyni anda edir.

//1️⃣ _configuration["Jwt:ExpireMinutes"] — BU NƏDİR?

//Bu, appsettings.json-dan bir dəyər oxuyur.

//Sənin config:

//"Jwt": {
//  "ExpireMinutes": 60
//}


//ASP.NET Core-da:

//_configuration["Jwt:ExpireMinutes"]


//deməkdir:

//Jwt bölməsinin içindəki ExpireMinutes dəyərini oxu

//⚠️ Burada gələn tip:

//string ("60")

//Çünki config həmişə string qaytarır.

//2️⃣ int.Parse(...) — NİYƏ LAZIMDIR?

//Sən token yaratarkən bunu yazırsan:

//DateTime.UtcNow.AddMinutes(expireMinutes)


//AddMinutes double / int istəyir, string yox.

//Ona görə:

//"60"  ❌
//60    ✅


//int.Parse string → int çevirir.

//3️⃣ ! (NULL-FORGIVING OPERATOR) — BU NƏDİR?

//Bu işarə compiler-ə yalan danışmaqdır 🙂
//Amma şüurlu yalandır.

//_configuration["Jwt:ExpireMinutes"]!


//deməkdir:

//“Mən bilirəm ki, bu dəyər null deyil, sən narahat olma.”

//Əgər config-də bu açar yoxdursa:

//runtime-da exception atılacaq

//bu normaldır, çünki auth düzgün qurulmayıb

//4️⃣ BU SƏTİRİN MƏNTİQİ BİR CÜMLƏ İLƏ

//Server appsettings.json-dan tokenin neçə dəqiqə yaşayacağını oxuyur və onu hesablamada istifadə etmək üçün int-ə çevirir.SƏNİN GÖRDÜYÜN SƏTİR
//var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);


//Bu sətir sadə görünür, amma 3 şey eyni anda edir.

//1️⃣ _configuration["Jwt:ExpireMinutes"] — BU NƏDİR?

//Bu, appsettings.json-dan bir dəyər oxuyur.

//Sənin config:

//"Jwt": {
//  "ExpireMinutes": 60
//}


//ASP.NET Core-da:

//_configuration["Jwt:ExpireMinutes"]


//deməkdir:

//Jwt bölməsinin içindəki ExpireMinutes dəyərini oxu

//⚠️ Burada gələn tip:

//string ("60")

//Çünki config həmişə string qaytarır.

//2️⃣ int.Parse(...) — NİYƏ LAZIMDIR?

//Sən token yaratarkən bunu yazırsan:

//DateTime.UtcNow.AddMinutes(expireMinutes)


//AddMinutes double / int istəyir, string yox.

//Ona görə:

//"60"  ❌
//60    ✅


//int.Parse string → int çevirir.

//3️⃣ ! (NULL-FORGIVING OPERATOR) — BU NƏDİR?

//Bu işarə compiler-ə yalan danışmaqdır 🙂
//Amma şüurlu yalandır.

//_configuration["Jwt:ExpireMinutes"]!


//deməkdir:

//“Mən bilirəm ki, bu dəyər null deyil, sən narahat olma.”

//Əgər config-də bu açar yoxdursa:

//runtime-da exception atılacaq

//bu normaldır, çünki auth düzgün qurulmayıb

//4️⃣ BU SƏTİRİN MƏNTİQİ BİR CÜMLƏ İLƏ

//Server appsettings.json-dan tokenin neçə dəqiqə yaşayacağını oxuyur və onu hesablamada istifadə etmək üçün int-ə çevirir.



//2️⃣ NİYƏ LAZIMDIR?

//Təsəvvür et ki, bunlar kodun içində olsaydı:

//var expireMinutes = 60;
//var jwtKey = "my-secret-key";
//var connectionString = "...";


//❌ Dəyişmək üçün:

//kodu dəyiş

//yenidən build et

//yenidən deploy et

//Configuration ilə:

//sadəcə config dəyişir

//kod toxunulmur

//3️⃣ HARADAN GƏLİR CONFIGURATION?

//ASP.NET Core-da configuration bir yerdən yox, bir neçə mənbədən oxunur:

//appsettings.json

//appsettings.Development.json

//Environment variables

//User secrets

//Command line

//Hamısı birləşdirilir → IConfiguration












//typed oxuma nedir?

//“Typed oxuma” (type-safe reading) — configuration-dan gələn string dəyəri avtomatik olaraq düzgün tipə (int, bool, TimeSpan və s.) çevirib almaqdır.

//Sadə müqayisə
//❌ Typed OLMAYAN (manual parse)
//var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);


//Problemlər:

//Dəyər null olarsa → exception

//Dəyər "abc" olarsa → exception

//Tip təhlükəsizliyi yoxdur

//✅ Typed OXUMA (tövsiyə olunan)
//var expireMinutes = _configuration.GetValue<int>("Jwt:ExpireMinutes");typex oxuma nedir?

//“Typed oxuma” (type-safe reading) — configuration-dan gələn string dəyəri avtomatik olaraq düzgün tipə (int, bool, TimeSpan və s.) çevirib almaqdır.

//Sadə müqayisə
//❌ Typed OLMAYAN (manual parse)
//var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);


//Problemlər:

//Dəyər null olarsa → exception

//Dəyər "abc" olarsa → exception

//Tip təhlükəsizliyi yoxdur

//✅ Typed OXUMA (tövsiyə olunan)
//var expireMinutes = _configuration.GetValue<int>("Jwt:ExpireMinutes");















//PRAKTİK YOL XƏRİTƏSİ (tövsiyə)

//1️⃣ İndi
//→ bu yazdığın custom auth-u bitir və başa düş

//2️⃣ Sonra (1–2 həftə)
//→ eyni sistemi ASP.NET Identity ilə yenidən yaz

//3️⃣ Onda
//→ fərqi 100% anlayacaqsan:

//“Identity nəyi mənim yerimə edir?”

//“Harada məni məhdudlaşdırır?”

//1 CÜMLƏLİK YEKUN

//Identity auth-u tez qurmaq üçündür, amma auth-u başa düşmək üçün əvvəl özün yazmalısan — sən hazırda düz yoldasan.

//İstəsən növbəti addımda:

//sənin mövcud auth-u Identity-yə necə miqrasiya etmək olar,

//ya da custom auth vs Identity müqayisəsini konkret maddələrlə edim.










//🔹 Enum error → client üçün

//UI nə göstərəcək?

//Hansı mesaj çıxacaq?

//Hansı flow davam edəcək?

//🔹 Log → sən və sistem üçün

//Nə baş verdi?

//Niyə baş verdi?

//Harada qırıldı?

//Bunlar eyni problemə baxır, amma fərqli tərəfdən.

//2️⃣ NİYƏ ENUM TƏK BAŞINA YETƏRLI DEYİL?

//Təsəvvür et:

//LoginStatus.InvalidPassword


//Client bunu görür — OK.

//Amma sən bunları bilmirsən:

//neçə dəfə olub?

//hansı IP-dən?

//brute-force var?

//saat neçədə çoxalır?

//➡️ Enum statik nəticədir, iz buraxmır.

//3️⃣ REAL SƏNARİ (çox vacib)
//Client-ə gedən:
//{
//    "status": "InvalidPassword"
//}

//Sənin log-da görməli olduğun:
//[Warning] Login failed. Invalid password. Email: x @x.com IP: 10.2.1.7


//Əgər log YOXDURSA:

//sistem kor olur

//debugging mümkün olmur

//security riskləri görünmür

//4️⃣ ƏN DÜZGÜN KOMBO (TÖVSİYƏ)

//Enum → nəticəni ifadə edir
//Log → hadisəni sənə izah edir

//Bu best practice-dir.

//5️⃣ NECƏ BİRLƏŞDİRİLİR? (qısa nümunə)
//if (user == null)
//{
//    _logger.LogWarning(
//        "Login failed. User not found. Email: {Email}",
//        dto.Email
//    );

//return LoginStatus.UserNotFound;
//}


//Client:

//enum alır

//Sən:

//log görürsən

//➡️ hamı razıdır.

//6️⃣ NƏ ZAMAN LOG YAZMAYA BİLƏRSƏN?

//ÇOX nadir hallarda:

//test layihəsi

//POC

//demo

//Real backend-də → yox.

//7️⃣ 1 CÜMLƏLİK QAYDA

//Enum istifadəçiyə cavabdır, log isə sistemin yaddaşıdır — biri o birinin yerini tutmur.

//İstəsən növbəti addımda:

//enum +log + HTTP status ideal kombinasiyasını,

//ya da login üçün tam log strategiyasını
//konkret kodla göstərə bilərəm.