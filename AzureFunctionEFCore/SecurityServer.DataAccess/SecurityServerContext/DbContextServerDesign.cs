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

            string connectionString = "Server=tcp:test-server-database.database.windows.net,1433;Initial Catalog=testdbdiiagedeux;Persist Security Info=False;User ID=CloudSAf42aaec6;Password=Azerty@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            optionsBuilder.UseSqlServer(connectionString);
            return new DbContextServer(optionsBuilder.Options);
        }
    }
}
