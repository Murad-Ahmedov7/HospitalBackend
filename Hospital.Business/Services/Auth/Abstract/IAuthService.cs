using Hospital.Business.DTOs.Auth.RequestDTOs;
using Hospital.Business.Enums;


namespace Hospital.Business.Services.Auth.Abstract
{
    public interface IAuthService
    {
        Task<bool> RegisterAsnyc(RegisterRequestDto dto);
        Task<(LoginStatus status, string? token,Guid? userId,int?expiresIn)> LoginAsnyc(LoginRequestDto dto);
    }
}
