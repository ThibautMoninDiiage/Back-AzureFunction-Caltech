using Microsoft.EntityFrameworkCore;

namespace AzureFunctionEFCore.Models
{
    public class DbContextEFCore : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbContextEFCore(DbContextOptions<DbContextEFCore> options) : base(options)
        {
        }
    }
}