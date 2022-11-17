using AzureFunction.Context.DbContextAZ;
using AzureFunction.Context.Models;
using AzureFunction.Repository.Interfaces;

namespace AzureFunction.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(DbContextServeur dbContext) : base(dbContext)
        {

        }
    }
}
