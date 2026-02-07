


using BCrypt.Net;
using System;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hospital.Entities.User
{





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
            if (string.IsNullOrWhiteSpace(fullName))
                throw new Exception("FullName cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email cannot be empty");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new Exception("PasswordHash cannot be empty");

            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Phone= phone;
        }


        public void ChangePassword(string newPassword)
        {
            if(string.IsNullOrWhiteSpace(newPassword))
              throw new Exception("New password cannot be empty");
            PasswordHash =BCrypt.Net.BCrypt.HashPassword(newPassword);

        }

        //public void ChangePhone(string? phone)
        //{
        //    if (phone != null && phone.Length < 7)
        //        throw new DomainException("Invalid phone number");

        //    Phone = phone;
        //}



        //        Sənin dizaynında entity constructor + private set + invariant-larla qorunduğu üçün entity-də Data Annotation əlavə dəyər vermir.
        //Data Annotation əsasən input validation üçündür, halbuki səndə input artıq DTO və service səviyyəsində tam yoxlanır.
        //Entity isə yalnız “obyekt mövcud ola bilərmi?” sualını cavablayan invariant-ları saxlayır; format, uzunluq və UI qaydaları entity-nin məsuliyyəti deyil.
        //Bu səbəbdən entity-də Data Annotation yazmamaq səhv deyil, şüurlu və təmiz arxitektura seçimidir.








        //        Invariant-i ona görə yazdın ki, sistemin “daxili doğruluğunu” qoruyasan.
        //Bu, frontend və DTO-dan asılı olmayan yeganə qatdır.

        //DAHA AÇIQ DESƏK
        //1️⃣ Frontend və DTO kənarı qoruyur

        //Onlar deyir:

        //“İstifadəçi belə data göndərə bilməz”

        //“API belə data qəbul etmir”

        //Amma bunlar hamısı giriş səviyyəsidir.

        //2️⃣ Invariant isə özünü qoruyur

        //Invariant deyir:

        //“Mən bu vəziyyətdə ümumiyyətlə mövcud ola bilmərəm.”

        //Yəni:

        //haradan gəlməsindən asılı olmayaraq

        //kim çağırmasından asılı olmayaraq

        //pis obyekt YARANA BİLMƏZ.



            //Invariant yalnız “entity bu dəyərsiz yaşaya bilməz” dediyin hallara yazılır.
            //“Olsa yaxşıdır” → invariant deyil.


           //“INVARIANT” ƏSLİNDƏ NƏ DEMƏKDİR?

           // Sadə dillə:

           // Invariant = dəyişməz qayda

           // Yəni:

           // obyekt yaradıldısa

           // ömrü boyu
           // BU QAYDA POZULA BİLMƏZ


        //!!!!!!!!!!! NURLAN MUELLIMDEN SORUS BU ENCAPLATION-I DUZ ETMISEMMMMMMMM VE  EFCORE-DA ISTINSA HALINI ARASIDR VE  QAYLDALRI BIRAZ DA BOL VE HER SENEDI TESDIQLE


        //!!!!!!!!!!!!CODDDA KOMENTLER YAZ


        //.empty istifade olunur?????(string.empty guid.empty )


        ///yazdigin qaydalari sual formatinda da yaz .
        ///

        //qaydani sorus muellimden kitab ve ya nese basqa bir sey

        //eger hardasa yeni qayda cixanda ilk once qayda yerinde gor yazmisam ona bax....


        //kodlara comment yazzzzzzzzzzzzzzzzzzzzzz

        //try catch yazzzzzzzzzzz ve testi sorus muellimeden

        //ve cv ,tedbir isi sorus/
        //kod suallari vere biler interview ve canli kod yamza gptsiz 


        //gpt ile nece kod yaz sohbetine bax gpt



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



    //!!!!!!!!!!!!!!! efcore ile bagli qaydalar var onlari unutma(DEFAULT CONSTRUCTOR PAREMTRIK NE VAXT ISTIFADE EDIR .VE INYE PRIVATE NIYE PAREMTRIK YAZDIQ)

    //!!!!!!!!!!FOLDER STRCUTRE SORUS KOHNE VERSIYASINI

    //    //Hospital.Domain
    //Hospital.Application
    //Hospital.Infrastructure
    //Hospital.API



    //Hospital.Entities   → Domain
    //Hospital.Business   → Application / Business Logic
    //Hospital.DataAccess → Infrastructure
    //Hospital.Api        → Presentation


    #region Qayda

    //Qeyd:HEMISE QAYDLARIN DUZGUNLUYUN GPTDE SORUS................






    #endregion
}
