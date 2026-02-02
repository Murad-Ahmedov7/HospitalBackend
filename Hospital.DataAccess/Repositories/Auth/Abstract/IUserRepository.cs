

using Hospital.Entities.User;

namespace Hospital.DataAccess.Repositories.Auth.Abstract
{
    public interface IUserRepository
    {
        Task AddAsync(User user);

        Task<User?> GetByEmailAsync(string email);
        Task SaveChangesAsync();
    }
}
