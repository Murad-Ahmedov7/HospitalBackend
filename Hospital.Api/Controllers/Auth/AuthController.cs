
using Hospital.Business.DTOs.Auth.RequestDTOs;
using Hospital.Business.DTOs.Auth.ResponseDTOs;
using Hospital.Business.Enums;
using Hospital.Business.Services.Auth.Abstract;

using Microsoft.AspNetCore.Mvc;


namespace Hospital.Api.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            await _authService.RegisterAsnyc(dto);
            return Ok();
        }
        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            //biz bu kodda deconstruction edirik.
            var (status,token, userId, expiresIn) = await _authService.LoginAsnyc(dto);



            if(status == LoginStatus.Success)
            {
                return Ok(new LoginResponseDto
                {
                    Token = token!,
                });
            }

            if (status == LoginStatus.InvalidCredentials)
            {
                return Unauthorized(new {message= "Email və ya şifrə yanlışdır." });
            }

            if (status == LoginStatus.AccountLocked)
            {
                return StatusCode(429, new { message = "Çox sayda uğursuz giriş cəhdi aşkarlandı. Zəhmət olmasa, bir müddət sonra yenidən cəhd edin." });
            }

            return StatusCode(500, new { message = "Gözlənilməz xəta baş verdi." });




            //!!!!!!!!!!!duran kimi bu sehv mesaj return etme qaydalarina bax.

        }
    }
}


//Deconstructing(deconstruction) —
//bir obyektin içindəki dəyərləri parçalayaraq ayrı-ayrı dəyişənlərə almaq deməkdir.


//biz bu kodda deconstruction edirik.


//4️⃣ BUNU AÇIQ YAZSAQ NECƏ GÖRÜNƏR?

//Bu sətir:

//var (token, userId, expiresIn) = await _authService.LoginAsnyc(dto);


//əslində buna bərabərdir:

//var temp = await _authService.LoginAsnyc(dto);

//string? token = temp.token;
//Guid? userId = temp.userId;
//int? expiresIn = temp.expiresIn;



//Biz property-lərə(result.token və s.) müraciət edə bilirik, çünki funksiya adlandırılmış tuple(named tuple) qaytarır.





//QAYDA(QIZIL QAYDA)

//Public API(controller) heç vaxt tuple qaytarmamalıdır — həmişə konkret DTO / class (yəni sabit response obyekti) qaytarmalıdır.

//NECƏ DÜŞÜNƏSƏN? (MENTAL MODEL)

//Tuple → daxili işlər üçündür
//(service → service, helper → helper)

//DTO / class → API müqaviləsidir
//(controller → client)

//PRAKTİK QAYDALAR(qısa siyahı)
//✅ Tuple istifadə ET

//private metodlarda

//eyni metodun içində

//sürətli daxili qaytarmalar üçün

//private (bool isValid, string reason) Validate(...)

//❌ Tuple istifadə ETMƏ

//controller response-da

//Swagger görünməli yerdə

//public API contract-da

//// YANLIŞ
//public IActionResult Login() => Ok((token, userId));

//✅ DTO istifadə ET
//public class LoginResponseDto
//{
//    public string Token { get; set; } = null!;
//    public Guid UserId { get; set; }
//    public int ExpiresIn { get; set; }
//}

//return Ok(loginResponseDto);

//1 CÜMLƏLİK YEKUN(bunu yaz)

//Tuple — daxili mexanizmdir, DTO isə API müqaviləsidir.





















//Yaxşı, bunu detallı və texniki izah edirəm ki, niyə new { ... } işləsə də uzunömürlü API üçün pis seçimdir.

//1️⃣ Anonim obyekt NƏDİR?
//return Ok(new { result.token, result.userId, result.expiresIn });


//Bu sətir:

//compile - time - da adlandırılmış müvəqqəti bir class yaradır

//bu class:

//adı yoxdur

//yalnız həmin method scope-u üçündür

//başqa yerdə istifadə edilə bilməz

//Yəni bu real model deyil, ad-hoc cavabdır.

//2️⃣ NİYƏ “WORKAROUND” SAYILIR?
//🔹 1. API CONTRACT YOXDUR

//API-lar müqavilə ilə yaşayır.

//DTO ilə:

//public class LoginResponseDto
//{
//    public string Token { get; set; }
//    public Guid UserId { get; set; }
//    public int ExpiresIn { get; set; }
//}


//Burada açıq-aşkar deyirsən:

//“Login endpoint-in response modeli BUDUR.”

//Anonim obyektlə isə:

//model kodda görünmür

//API contract gizlidir

//başqa developer bu response-u tapıb oxuya bilmir

//🔹 2. VERSIYALAMA PROBLEMİ

//Tutaq ki sabah:

//refreshToken əlavə etmək istəyirsən

//ya roles qaytarmaq istəyirsən

//DTO ilə:

//yeni property əlavə edirsən

//compile-time sənə kömək edir

//Anonim obyektlə:

//hər controller-də new { ... } dəyişməlisən

//səssiz breaking change riski var

//🔹 3. REUSE MÜMKÜN DEYİL

//DTO:

//LoginResponseDto


//test-də istifadə olunur

//başqa endpoint-də reuse olunur

//documentation-da görünür

//Anonim obyekt:

//yalnız 1 yerdə yaşayır

//copy–paste məcburiyyəti yaradır

//DRY prinsipini pozur

//🔹 4. SWAGGER / OPENAPI PROBLEMLƏRI

//Swagger:

//DTO → tam schema

//anonim obyekt → təxmin

//Bu nəticəyə gətirir:

//yanlış schema

//incomplete documentation

//frontend üçün qeyri-müəyyənlik

//🔹 5. TEST YAZMAQ ÇƏTİNLƏŞİR

//DTO ilə:

//Assert.Equal(expected.UserId, response.UserId);


//Anonim obyektlə:

//type yoxdur

//cast lazımdır

//reflection / dynamic lazım olur

//➡️ Testlər çirklənir.

//3️⃣ NƏ ZAMAN ANONİM OBYEKT NORMALDIR?
//✅ OK sayılır, əgər:

//POC / demo yazırsansa

//internal tool -dur

//debug məqsədlidir

//1 dəfəlik response-dur

//❌ OK DEYİL, əgər:

//public API -dır

//uzunömürlüdür

//versioning olacaqsa

//frontend komandası varsa

//4️⃣ ƏN DÜZGÜN PRAKTİKA (QIZIL QAYDA)

//Controller response = DTO / class
//Anonim obyekt = müvəqqəti çıxış yolu

//5️⃣ 1 CÜMLƏLİK YEKUN

//new { ... } işləyir, amma API müqaviləsi yaratmır; uzunömürlü və genişlənən API-lar üçün bu ciddi dizayn riskidir.










//🔒 API MÜQAVİLƏSİ ÜZRƏ QIZIL QAYDALAR
//1️⃣ API müqavilə nədir?

//API müqavilə — client ilə server arasında dəyişməməli olan request/response razılaşmasıdır.

//2️⃣ Controller NƏ QAYTARMALIDIR?

//✅ Həmişə DTO / class
//❌ Heç vaxt tuple
//❌ Heç vaxt new { ... }(anonim obyekt)

//return Ok(LoginResponseDto);

//3️⃣ DTO nədir?

//DTO — API-nin rəsmi response modelidir (müqavilənin kodlaşdırılmış forması).

//DTO:

//Swagger - də görünür

//Frontend üçün aydındır

//Uzunömürlüdür

//Dəyişiklikləri izləmək olur

//4️⃣ Tuple HARADA olar?

//✅ Service daxilində
//✅ Private metodlarda

//❌ Controller response-da
//❌ Public API-də

//// OK
//private (bool isValid, string reason) Validate()

//5️⃣ Anonim obyekt(new { ... }) nə vaxt olar?

//✅ Demo
//✅ Debug
//✅ Müvəqqəti workaround

//❌ Production
//❌ Uzunömürlü API
//❌ Public endpoint

//6️⃣ Swagger sənə nə deyir?

//Swagger düzgün schema göstərə bilmirsə → API müqavilən yanlışdır.

//Bu xəbərdarlıqdır, təsadüf deyil.

//7️⃣ Controller-in məsuliyyəti

//HTTP status qaytarmaq

//DTO qaytarmaq

//Business logic bilməmək

//8️⃣ QISA FORMUL (yadda saxla)
//Service → tuple ola bilər
//Controller → DTO olmalıdır
//Swagger → DTO istəyir
//API → müqavilə ilə yaşayır

//9️⃣ 1 CÜMLƏLİK YEKUN (bunu yaz)

//Public API heç vaxt tuple və ya anonim obyekt qaytarmamalıdır — yalnız sabit DTO ilə müqavilə qurmalıdır.