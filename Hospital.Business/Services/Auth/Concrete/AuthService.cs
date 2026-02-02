using Hospital.Business.DTOs.Auth;
using Hospital.Business.Services.Auth.Abstract;
using Hospital.DataAccess.Repositories.Auth.Abstract;
using Hospital.Entities.User;

namespace Hospital.Business.Services.Auth.Concrete
{
    
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> RegisterAsnyc(RegisterRequestDto dto)
        {
            var user=new User(dto.FullName,dto.Email,dto.Password,dto.Phone);

           await _userRepository.AddAsync(user);

           await _userRepository.SaveChangesAsync();


            return true;
        }

        public async Task <bool> LoginAsnyc(LoginRequestDto dto)
        {
            var user= await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || user?.Password!=dto.Password) { 
                return false;
            }

            return true;
        }
    }
}
