using Azure.Core;
using Hospital.Business.DTOs.Auth.RequestDTOs;
using Hospital.Business.Enums.Auth;
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

        
        public async Task<ChangePasswordStatus> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto)
        {
            var user=await _userRepository.GetByIdAsync(userId);

            if (user == null) return ChangePasswordStatus.UserNotFound;


            if (user.PasswordHash == null) return ChangePasswordStatus.GoogleAccount; 


            //Əgər istifadəçi Google ilə qeydiyyatdan keçibsə, onun parol hash-i null olacaq və bu halda parol dəyişdirilməsinə icazə verilməməlidir.


            //burada return-i digeri enum ile muqayise et ferqe bax.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);

            var samePassword = BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash);



            if (!passwordValid) return ChangePasswordStatus.WrongPassword;
            

            if (samePassword)  return ChangePasswordStatus.PasswordUnchanged;




            user.ChangePassword(dto.NewPassword);

            await _userRepository.SaveChangesAsync();

            return ChangePasswordStatus.Success;
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


                //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

                    //Yəni sən token yaradanda bunu yazmısan:

                    //new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())

                    //ASP.NET Core bunu avtomatik olaraq çevirib:
                    //ClaimTypes.NameIdentifier
                    //➡️ Ona görə də ayrıca NameIdentifier yazmasan da işləyib.
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





  //  {



    //jwt decode
}

//kodu cox tehlukesiz yazmaq yorurrr


    