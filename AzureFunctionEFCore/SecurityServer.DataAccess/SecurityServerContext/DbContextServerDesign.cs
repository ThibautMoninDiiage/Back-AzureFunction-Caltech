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

            string connectionString = "Server=tcp:preprod-server-database-groupe2b.database.windows.net,1433;Initial Catalog=pre-prod-sql-server;Persist Security Info=False;User ID=CloudSAa18a0c39;Password=JgR7hp2AB8/eua;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            optionsBuilder.UseSqlServer(connectionString);
            return new DbContextServer(optionsBuilder.Options);
        }
    }
}
