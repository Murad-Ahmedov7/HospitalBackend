
using Hospital.Business.DTOs.Auth;


namespace Hospital.Business.Services.Auth.Abstract
{
    public interface IAuthService
    {
        Task<bool> RegisterAsnyc(RegisterRequestDto dto);
        Task<bool> LoginAsnyc(LoginRequestDto dto);
    }
}
