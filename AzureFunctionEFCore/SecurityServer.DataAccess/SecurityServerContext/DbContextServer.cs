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
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Grant> Grants { get; set; }
        public DbSet<Tristan> Tristans { get; set; }
        

        public DbContextServer(DbContextOptions<DbContextServer> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUserRole>().HasKey(a => new { a.RoleId, a.UserId, a.ApplicationId });
            modelBuilder.Entity<Grant>().HasKey(g => new { g.UserId, g.ApplicationId });
            modelBuilder.Entity<Claim>().HasMany(c => c.Applications);
            modelBuilder.Entity<Claim>().HasMany(c => c.Users);
            modelBuilder.Entity<User>().HasMany(u => u.Claims);
            modelBuilder.Entity<Application>().HasMany(a => a.Claims);
        }
    }
}
