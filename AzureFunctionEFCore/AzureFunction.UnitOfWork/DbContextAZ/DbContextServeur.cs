using AzureFunction.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureFunction.UnitOfWork.DbContextAZ
{
    public class DbContextServeur : DbContext
    {
        public DbSet<Role>? Roles { get; set; }
        public DbSet<User>? Users { get; set; }

        public DbContextServeur(DbContextOptions<DbContextServeur> options) : base(options)
        {
        }
    }
}
