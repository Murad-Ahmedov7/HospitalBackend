

using Azure.Core;
using Hospital.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Numerics;

namespace Hospital.Business.DTOs.Auth.RequestDTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(2)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;


    }
}


//ƏGƏR BURADA DAYANSAN NƏ OLAR? ❌

//Əgər sən:

//yalnız DTO-ya güvənsən

//Entity-də yoxlama ETMƏSƏN

//onda:

//Controller bypass ediləndə (test, background job)

//sistemə qaydasız data girə bilər

//Ona görə:

//🔑 DTO validation = ilk qapı
//🔑 Entity validation = son qapı

//DÜZGÜN BALANS (STRONG JUNIOR ÜÇÜN)
//DTO-da ✔️

//[Required]

//[MinLength]

//[EmailAddress]

//[Phone]

//Entity - də ✔️

//null / empty check

//invariant-lar

//domain qaydaları

//1 CÜMLƏLİK QAYDA (YAZ)

//DTO-da validation format üçündür,
//Entity-də validation qayda üçündür.






//MÜQAYİSƏ(çox aydın)
//string? a = null;
//Console.WriteLine(a.Length); // ⚠️ warning + 💥 exception

//string b = null!;
//Console.WriteLine(b.Length); // ❌ warning YOX + 💥 exception


//Runtime nəticə EYNİDİR:

//NullReferenceException


//Fərq yalnız compile-time-dadır.

//1 CÜMLƏLİK QAYDA (YAZ)

//null! exception ATMIR,
//null üzərində .Length exception ATIR.


//| Yazılış          | Compile - time                    | Runtime |
//| ---------------- | --------------------------------  | -------------- |
//| `string?`        | ⚠️ xəbərdar edir(warning)         | null ola bilər |
//| `string = null!` | ❌ susur                          | null ola bilər |
//| `string`         | ❌ susur / warning constructor-da | null ola bilər |


//string? → mən risk qəbul edirəm

//string = null! → mən söz verirəm

//string → mən nə etdiyimi bilmirəm 😄

//Bu sualla sən artıq null mövzusunun SON pilləsinə ça


//🔹 ? — BUNUN ADI NƏDİR?
//Nullable Reference Type (qısa: nullable operator)
//string? name;


//Rəsmi məna:

//“Bu reference type null OLA BİLƏR.”

//Compile-time anlayışdır

//Compiler null check tələb edir

//Runtime davranışı dəyişmir

//📌 Düzgün termin:

//nullable reference type (?)

//🔹 ! (null!) — BUNUN ADI NƏDİR?
//Null-forgiving operator
//string name = null!;


//Rəsmi məna:

//“Bu null deyil deyə compiler-ə söz verirəm.”

//YALNIZ compile-time üçündür

//Warning-i susdurur

//Runtime təhlükəsizliyi YOXDUR

//📌 Düzgün termin:

//null - forgiving operator (!)

//🧠 QISA YADDAŞ CƏDVƏLİ
//Yazılış	Rəsmi adı	Nə edir
//string?	Nullable reference type	Null ola bilər deyir
//null!	Null-forgiving operator	Warning-i susdurur






//BƏLİ.Tam düzgün tutmusan. 👍
//İndi bunu qısa və qəti bağlayıram ki, beynində “rule” kimi qalsın.

//DƏQİQ HƏQİQƏT
//1️⃣ null! NƏ EDİR?

//Compile-time warning-i SUSDURUR

//Runtime-u QORUMUR

//Əgər dəyər sonradan null qalarsa → runtime-da exception ATA BİLƏR

//👉 Yəni:

//Compile susur, amma runtime səni bağışlamır.

//2️⃣ ONA GÖRƏ NƏ ETMƏLİYİK?

//👉 DTO validation yazmalıyıq.

//Çünki:

//yanlış data daha erkən tutulur

//runtime-a qədər getmir

//kod “gec partlamır”

//DÜZGÜN ZƏNCİR (bunu yadda saxla)

//1️⃣ DTO validation

//[Required]
//[MinLength(2)]
//public string FullName { get; set; } = null!;


//➡️ səhv data → request kəsilir (400)

//2️⃣ Entity constructor validation

//if (string.IsNullOrWhiteSpace(fullName))
//    throw new ArgumentException();


//➡️ hardan çağırılsa da → qəti exception

//3️⃣ null!
//➡️ yalnız compiler üçün
//➡️ təhlükəsizlik aləti DEYİL

//1 CÜMLƏLİK QAYDA (YAZ)

//Compile-i susdurmaq təhlükəsizlik deyil,
//təhlükəsizlik DTO və Entity validation-dır.

//SON HÖKM

//✔️ Bəli, compile susdursa da runtime-da exception ola bilər
//✔️ DTO validation mütləq lazımdır
//✔️ Entity validation son qaladır
//✔️ Sən artıq mövzunu düzgün səviyyədə tutmusan




//2️⃣ DTO validation VARSA NƏ OLUR?
//[Required]
//[MinLength(2)]
//public string FullName { get; set; } = null!;


//Bu halda:

//səhv data controller-ə girməmiş kəsilir

//ASP.NET Core avtomatik 400 BadRequest qaytarır

//Service / Entity boşuna işləməz





//SƏN HAQLISAN: “MƏN CONSTRUCTOR YAZMAMIŞAM”

//BƏLİ. Sən yazmamısan.
//AMMA 👇

//🔴 C# NƏ EDİR?

//Əgər sən constructor yazmırsansa, C# AUTOMATİK OLARAQ bunu yaradır:

//public Dto()
//{
//}


//Buna deyilir: implicit (default) constructor.

//Yəni:

//Constructor VAR, sadəcə sən onu görmürsən.

//COMPILER NİYƏ “constructor-dan çıxanda” DEYİR?

//Bu koda baxaq:

//public class Dto
//{
//    public string FullName { get; set; }
//}


//Compiler belə düşünür:

//FullName → string (non-nullable)

//Deməli null OLMAMALIDIR

//Constructor-a baxır:

//implicit constructor var ✔️

//içində FullName-a dəyər verilib? ❌

//➡️ Constructor bitəndə FullName == null

//VƏ ONA GÖRƏ WARNING VERİR 👇

//Non-nullable property 'FullName' must contain a non-null value
//when exiting constructor.


//Mövzu         required            string	[Required]
//Kimindir      C# dili	            ASP.NET
//Zaman	        Compile-time	    Runtime
//Harada	    Obyekt yaradılarkən	Request qəbul edilərkən
//API validation ❌	                ✅
//ModelState	❌	                ✅
//Domain üçün	⚠️ (nadir)	        ❌          
//DTO üçün	    ⚠️ (az)	            ✅


//required — obyekt yaratma qaydasıdır.
//[Required] — request yoxlamasıdır.





//required yalnız object initializer mərhələsində işləyir;
//initializer yoxdursa, yoxlama da yoxdur.



//Object initializer — obyekti new edəndə
//constructor - dan sonra property-ləri { } ilə doldurma üsuludur.

//1️⃣ ƏN SADƏ NÜMUNƏ
//var u = new User
//        {
//            FullName = "Ali",
//            Email = "a@b.com"
//        };


//👉 Bu { ... } hissəyə object initializer deyilir.

//2️⃣ BU OLMASAYDI NECƏ OLARDI?
//var u = new User();
//u.FullName = "Ali";
//u.Email = "a@b.com";


//Object initializer sadəcə qısa və səliqəli yazılışdır.



//SƏNİN ENTITY-DƏ NİYƏ required İŞLƏMİR? (yekun)

//Çünki sən:

//private set istifadə edirsən ✔️

//parametrli constructor istifadə edirsən ✔️

//object initializer-i qapadırsan ✔️

//➡️ Bu halda required-in işləməsi MÜMKÜN DEYİL və LAZIM DA DEYİL.

//1 CÜMLƏLİK QAYDA (yaz bunu)

//required yalnız public set +object initializer olan modellər üçündür;
//Entity - lərdə constructor bunu əvəz edir.