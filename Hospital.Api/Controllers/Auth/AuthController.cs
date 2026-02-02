using Hospital.Business.DTOs.Auth;
using Hospital.Business.Services.Auth.Abstract;
using Microsoft.AspNetCore.Http;
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
            var result=await _authService.LoginAsnyc(dto);

            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
