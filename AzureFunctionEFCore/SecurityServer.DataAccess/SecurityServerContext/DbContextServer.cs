using Microsoft.EntityFrameworkCore;
using SecurityServer.Models.Models;

namespace SecurityServer.DataAccess.SecurityServerContext
{
    public class DbContextServer : DbContext
    {
        public DbSet<Role>? Roles { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<Application> Applications { get; set; }

        public DbContextServer(DbContextOptions<DbContextServer> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(t => t.IdRole);
            modelBuilder.Entity<Application>().HasMany(u => u.Users);
        }
    }
}
