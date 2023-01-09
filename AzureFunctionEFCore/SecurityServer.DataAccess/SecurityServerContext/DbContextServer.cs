using Microsoft.EntityFrameworkCore;
using SecurityServer.Models.Models;

namespace SecurityServer.DataAccess.SecurityServerContext
{
    public class DbContextServer : DbContext
    {
        public DbSet<Role>? Roles { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public DbContextServer(DbContextOptions<DbContextServer> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserRole>().HasKey(a => new { a.RoleId, a.UserId, a.ApplicationId });
            modelBuilder.Entity<ApplicationUserRole>().HasOne(a => a.Role).WithMany(r => r.ApplicationUserRoles).HasForeignKey(a => a.RoleId);
            modelBuilder.Entity<ApplicationUserRole>().HasOne(a => a.User).WithMany(r => r.ApplicationUserRoles).HasForeignKey(a => a.UserId);
            modelBuilder.Entity<ApplicationUserRole>().HasOne(a => a.Application).WithMany(r => r.ApplicationUserRoles).HasForeignKey(a => a.ApplicationId);
        }
    }
}
