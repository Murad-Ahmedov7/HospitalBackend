
using Hospital.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Hospital.DataAccess
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; private set; }

    }
}
