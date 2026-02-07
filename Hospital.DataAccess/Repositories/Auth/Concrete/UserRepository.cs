using Hospital.DataAccess.Repositories.Auth.Abstract;
using Hospital.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Hospital.DataAccess.Repositories.Auth.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly HospitalDbContext _context;


        public UserRepository (HospitalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

  

        public async Task <User?> GetByEmailAsync(string email)
        {
            return 
                await _context.Users.FirstOrDefaultAsync(x=>x.Email == email);

        }

        public async Task <User?> GetByIdAsync(Guid id)
        {
            return 
                await _context.Users.FirstOrDefaultAsync(x=>x.Id== id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
