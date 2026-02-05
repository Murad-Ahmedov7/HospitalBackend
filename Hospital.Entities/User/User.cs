


namespace Hospital.Entities.User
{

    //Code Comment:::

    // DOMAIN RULE:
    // This entity enforces data correctness at two levels:
    // - Compile-time constraints (required, nullable reference types)
    // - Runtime validation via constructor and domain methods
    // This prevents invalid domain states from ever existing.



    public class User
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public string PasswordHash { get; private set; }


        private User() { }

        public User(string fullName, string email,string passwordHash,string phone)
        {
            //if (string.IsNullOrWhiteSpace(fullName))
            //    throw new DomainException("FullName is required");

            //if (string.IsNullOrWhiteSpace(email))
            //    throw new DomainException("Email is required");

            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Phone= phone;
        }

        //public void ChangePhone(string? phone)
        //{
        //    if (phone != null && phone.Length < 7)
        //        throw new DomainException("Invalid phone number");

        //    Phone = phone;
        //}





        //exceptionlari global middleware ile yaz./


        //men hem test, hem middleware, hem de log yazmaq iteyirem ve ci/cd de olsun

        //sonradan github-da folder adi deyisme 

        //solid prinspi ve her kod parcasinda comment yaz.

        //readme file ve github daha yaxsi et


        //typescript or javascript yazim proyekti?

        //project referenclar ????????? nedi????????

        //backend microservices and frontend microfrontend

        //blob storage redis?


        //testin project reference-i ne olsun?


        //bu yeni folder structure icinde olan adlari ozun basqa proyektde yazacam....(gptde hospital backend 1-in axirina da bax.)  


        //    1️⃣ ANALİZ

        //Sual:

        //“Bir strong junior üçün hansısı daha yaxşıdır?”

        //Variantlar faktiki bunlardır:

        //A) UseCase / Execute / Clean-heavy

        //B) IService / Service / sadə layer

        //2️⃣ VALIDATION – Dürüst cavab
        //✅ Strong junior üçün ən yaxşısı: B variantı

        //IAuthService / AuthService yanaşması

        //Niyə? (bunu nurlan muellimden sorus kohne usl hemise men yazdigin folder yoxsa indiki)(HANSI ILE YAZIM HAL HAZIRDA YENIDEN KOHNEYE KECDIM)

        //Abstract
        //Concrete   hele ki bunlari folder adi kimi yazma.
    }



    //men servicelari yazanda qaydalari yoxlayirdim o sehv idi....(yeni validatioblari)


    //Parametrli constructor varsa, default constructor avtomatik YOXDUR.
    //Lazımdırsa, sən özün yazmalısan.




    #region Qayda

    //Qeyd:HEMISE QAYDLARIN DUZGUNLUYUN GPTDE SORUS................





    //DÜZƏLDİLMİŞ QAYDA(FirstAsync() DAXİL EDİLƏRƏK )

    //Birinci hal(constructor-suz entity):

    //Əgər mən entity-ni heç bir constructor yazmadan bu formada yaratsam, bu təhlükəlidir, çünki:

    //1)C# avtomatik olaraq public default constructor yaradır

    //2)Bu constructor həm application kodu, həm də EF Core tərəfindən istifadə oluna bilir

    //3)Application tərəfində bu mümkündür:

    //var user = new User(); // boş, etibarsız entity



    //4)Service qatında entity çağırılarkən:

    //FullName, Email və s. null / empty ola bilər

    //entity özü bunu qarşısını ala bilmir


    //5)Eyni zamanda, EF Core DB-dən data oxuyanda bu kod işləyir:

    //var user = await _context.Users.FirstAsync();


    //6)Bu anda EF Core:

    //public default constructor - u çağırır

    //əvvəl boş User obyekti yaradır

    //sonra DB-dən oxunan datanı property-lərə doldurur




    //7)Problem buradadır ki:

    //bu default constructor hamı üçün açıqdır

    //EF Core üçün lazım olan mexanizm

    //kənar kod üçün də istismar edilə bilər



    //8)Nəticədə:

    //entity həm business flow zamanı

    //həm də istənilən başqa kod tərəfindən

    //qaydasız və nəzarətsiz yaradıla bilir



    //Nəticə:
    //Constructor - suz entity təhlükəlidir, çünki:

    //C# avtomatik public default constructor yaradır

    //Bu constructor EF Core üçün lazım olsa da

    //eyni zamanda kənar kodun da entity-ni boş yaratmasına icazə verir



    //entity öz qaydalarını qorumaq gücündə olmur




    //QISA FORMADA (YADDA SAXLA)

    //FirstAsync() EF Core-un entity-ni DB-dən bərpa etməsi üçün
    //default constructor-dan istifadə edir.
    //Əgər bu constructor public -dirsə,
    //eyni qapı kənar kod üçün də açıq olur — problem də buradan başlayır.

    //1 CÜMLƏLİK YEKUN
    //Default constructor EF Core üçün lazımdır,
    //amma public olarsa domain üçün təhlükəlidir.
    //Ona görə onu biz özümüz private yazırıq.






    //Çox yaxşı nöqtəyə toxundun. İndi EF Core üçün lazım olan mexanizmin kənar kod tərəfindən NECƏ istismar edilə bildiyini tam real, konkret nümunə ilə göstərirəm.



    //Qeyd:::::QISA CAVAB

    //👉 public set olmasa,
    //👉 constructor public olsa belə,

    //property-ni birbaşa dəyişə BİLMƏZSƏN. ✔️
    //Bu hissədə sən haqlısan. ters hali da ola biler.


    //SƏHNƏ: constructor - suz entity(təhlükəli vəziyyət)
    //public class User
    //{
    //    public Guid Id { get; set; }
    //    public string FullName { get; set; }
    //    public string Email { get; set; }
    //}




    //Burada:

    //heç bir constructor yoxdur

    //C# avtomatik public User() yaradır

    //EF Core bunu DB-dən oxuyanda istifadə edir (OK)

    //amma problem buradan başlayır

    //❌ NÜMUNƏ 1 — Kənar service EF Core kimi davranır
    //public class FakeUserService
    //{
    //    public User CreateBrokenUser()
    //    {
    //        var user = new User();   // ⚠️ EF Core mexanizmi istismar olunur
    //        user.Email = "test@mail.com";
    //        // FullName YOXDUR

    //        return user;
    //    }
    //}


    //Bu kod:

    //texniki olaraq tam qanunidir

    //compile-time error YOXDUR

    //runtime error YOXDUR

    //Amma:

    //domain qaydası pozulub

    //adı olmayan user sistemə daxil oldu

    //👉 EF Core üçün açıq olan qapı, service tərəfindən sui-istifadə edildi.



    //❌ NÜMUNƏ 2 — Test və ya background job səssizcə pozur
    //public async Task ImportUsersAsync()
    //{
    //    var user = new User();  // ⚠️ boş entity
    //    user.Email = "import@mail.com";

    //    await _userRepository.AddAsync(user);
    //}


    //Burada:

    //“mən tələsirəm” deyən developer

    //validation yazmağı unudur

    //DB-yə yarımçıq user gedir

    //Bu cür bug-lar:

    //illərlə production-da yaşaya bilər

    //tapmaq çox çətindir

    //❌ NÜMUNƏ 3 — Controller birbaşa model bağlayır (ən təhlükəlisi)
    //[HttpPost]
    //public async Task<IActionResult> Create(User user)
    //{
    //    await _repo.AddAsync(user); // ⚠️
    //    return Ok();
    //}


    //Əgər frontend belə göndərsə:

    //{
    //    "email": "hack@mail.com"
    //}


    //Nəticə:

    //FullName = null

    //amma User yarandı

    //DB-yə getdi

    //Bu real production fəlakətidir.     







    //BÜTÜN BU PROBLEMLƏR NİYƏ MÜMKÜNDÜR?

    //Çünki:

    //EF Core üçün lazım olan public default constructor

    //hamı üçün açıqdır

    //entity özünü qorumur

    //EYNİ SCENARİ — DÜZGÜN DİZAYNLA
    //public class User
    //{
    //    public Guid Id { get; private set; }
    //    public string FullName { get; private set; }
    //    public string Email { get; private set; }

    //    private User() { } // yalnız EF Core

    //    public User(string fullName, string email)
    //    {
    //        if (string.IsNullOrWhiteSpace(fullName))
    //            throw new DomainException();

    //        if (string.IsNullOrWhiteSpace(email))
    //            throw new DomainException();

    //        Id = Guid.NewGuid();
    //        FullName = fullName;
    //        Email = email;
    //    }
    //}


    //İndi:

    //new User();                 // ❌ compile-time error
    //new User(null, "a@mail");   // ❌ runtime error


    //Yəni:

    //EF Core işləyir ✅

    //Kənar kod istismar edə bilmir ❌

    //Domain qorunur ✅

    //1 CÜMLƏLİK TAM YEKUN

    //EF Core üçün açıq qoyulan qapı public olarsa,
    //kənar kod mütləq bir gün o qapıdan girəcək.
    //Ona görə həmin qapı private olmalıdır.

    //Bu nöqtəni başa düşdünsə, artıq constructor mövzusunun ən kritik hissəsini tutmusan.


    //Encapsulation = obyektin daxili vəziyyətini gizlətmək
    //və onu yalnız icazəli yollarla dəyişməyə icazə verməkdir.





    //Bəli.Tam dəqiq tutmusan.
    //Bu nümunədə İKİ AYRI, amma EYNİ DƏRƏCƏDƏ KRİTİK qayda pozulur. İndi bunu rəsmi şəkildə ayıraq.

    //BU KODDA POZULAN 2 ƏSAS QAYDA
    //var user = new User();   // ⚠️
    //user.Email = "test@mail.com"; // ⚠️

    //❌ QAYDA 1 — Entity yanlış vəziyyətdə YARANIR
    //var user = new User();

    //Pozulan prinsip:

    //Entity heç vaxt etibarsız vəziyyətdə mövcud olmamalıdır.

    //Bu anda:

    //FullName = null

    //Email = null

    //Amma User VAR

    //Bu:

    //constructor invariant-ın pozulmasıdır

    //domain-in “varlıq qaydası”nın pozulmasıdır

    //Yəni:

    //Yanlış User yaranır

    //❌ QAYDA 2 — Encapsulation pozulur (qaydasız dəyişiklik)
    //user.Email = "test@mail.com";

    //Pozulan prinsip:

    //Entity - nin daxili vəziyyəti yalnız öz qaydaları ilə dəyişdirilə bilər.

    //Burada:

    //heç bir validation yoxdur

    //heç bir domain qaydası işləmir

    //istənilən dəyər verilə bilər

    //Bu:

    //encapsulation breach-dir

    //domain-in nəzarətsiz qalmasıdır

    //Yəni:

    //Yanlış User ÜSTƏLİK qaydasız dəyişdirilir

    //BU NİYƏ XÜSUSİ TƏHLÜKƏLİDİR?

    //Çünki bu iki səhv bir-birini gücləndirir:

    //❌ əvvəl boş, etibarsız entity yaranır

    //❌ sonra istənilən yerdən parça-parça doldurulur

    //Bu model:

    //real layihələrdə ən çox bug yaradan modeldir

    //tapılması çətin problemlər yaradır

    //domain-in “həqiqət mərkəzi” olmasını məhv edir






    //DÜZGÜN FORMULYASİYA

    //Default (parameterless) constructor EF Core-un
    //DB-dən oxuduğu məlumatı C# obyektinə çevirməsi (materialize etməsi) üçün lazımdır.

    //.Users yazmağın özü constructor-u çağırmır.
    //Constructor məlumat OXUNANDA işə düşür.

    //ADDIM-ADDIM NƏ BAŞ VERİR?
    //1️⃣ Sən bunu yazırsan:
    //_context.Users


    //Bu hələ:

    //DB - yə getmir ❌

    //constructor çağırmır ❌
    //Sadəcə query obyektidir.

    //2️⃣ Sən bunu yazanda:
    //var user = await _context.Users.FirstAsync();


    //Bu anda EF Core:

    //DB - yə SELECT göndərir

    //Boş User obyekti yaradır → default constructor

    //DB-dən oxunan dəyərləri property-lərə doldurur

    //Hazır obyekti qaytarır

    //📌 Default constructor məhz burada lazımdır.




    //NƏ ÜÇÜN LAZIMDIR? (1 cümlə)

    //EF Core yeni entity yaratmır,
    //DB-də mövcud olan entity-ni “bərpa edir”.

    //Bərpa üçün:

    //boş obyekt lazımdır

    //ona görə default constructor lazımdır

    //ÇOX VACİB DƏQİQLƏŞDİRMƏ

    //❌ Yanlış düşüncə:

    //“Default constructor .Users yazmaq üçündür”

    //✅ Düzgün düşüncə:

    //Default constructor .FirstAsync(), .ToListAsync(), .SingleAsync()
    //kimi OXUMA əməliyyatları üçündür

    //1 CÜMLƏLİK TAM YEKUN

    //Default constructor EF Core-un DB-dən oxuduğu məlumatı obyektə çevirməsi üçün lazımdır;
    //.Users sadəcə başlanğıcdır, constructor oxuma zamanı işə düşür.
    #endregion
}
