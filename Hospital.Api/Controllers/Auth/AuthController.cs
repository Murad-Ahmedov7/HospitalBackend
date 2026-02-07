
using Hospital.Business.DTOs.Auth.RequestDTOs;
using Hospital.Business.DTOs.Auth.ResponseDTOs;
using Hospital.Business.Enums.Auth;
using Hospital.Business.Services.Auth.Abstract;
using Hospital.DataAccess.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Security.Claims;


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



         

        }

        [Authorize]
        [HttpPost("change-password")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse( userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var result=await _authService.ChangePasswordAsync(userId, dto);




            if(result==ChangePasswordStatus.GoogleAccount) return BadRequest(new { message = "Google hesabı ilə qeydiyyatdan keçmiş istifadəçilər şifrəni dəyişə bilməzlər." });

            if(result==ChangePasswordStatus.UserNotFound) return NotFound( new { message = "İstifadəçi tapılmadı." });

            if(result==ChangePasswordStatus.WrongPassword) return BadRequest( new { message = "Cari şifrə yanlışdır." });

            if(result==ChangePasswordStatus.PasswordUnchanged) return BadRequest( new { message = "Yeni şifrə cari şifrədən fərqli olmalıdır." });

            return Ok( new { message = "Şifrə uğurla dəyişdirildi." });

        }





    }
}










