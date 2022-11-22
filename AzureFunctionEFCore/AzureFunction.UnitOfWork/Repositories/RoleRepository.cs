using AzureFunction.Contract;
using AzureFunction.Contract.Repositories;
using AzureFunction.Models;
using AzureFunction.UnitOfWork.DbContextAZ;

namespace AzureFunction.UnitOfWork.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(DbContextServeur dbContext) : base(dbContext)
        {

        }
    }
}
