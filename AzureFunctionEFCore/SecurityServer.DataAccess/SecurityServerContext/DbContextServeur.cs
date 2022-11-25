using Microsoft.EntityFrameworkCore;
using SecurityServer.Models.Models;

namespace SecurityServer.DataAccess.SecurityServerContext
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
