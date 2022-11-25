using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecurityServer.DataAccess.SecurityServerContext
{
    public class DbContextServerDesign : IDesignTimeDbContextFactory<DbContextServer>
    {
        public DbContextServer CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextServer>();

            //string connectionString = @"";

            string connectionString = "Server=tcp:caltech.database.windows.net,1433;Initial Catalog=SecurityServerDB;Persist Security Info=False;User ID=superadmin;Password=Azerty@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            optionsBuilder.UseSqlServer(connectionString);
            return new DbContextServer(optionsBuilder.Options);
        }
    }
}
