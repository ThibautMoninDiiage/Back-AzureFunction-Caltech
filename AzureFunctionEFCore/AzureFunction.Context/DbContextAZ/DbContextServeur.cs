using AzureFunction.Context.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureFunction.Context.DbContextAZ
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
