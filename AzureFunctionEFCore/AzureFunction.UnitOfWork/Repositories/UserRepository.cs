using AzureFunction.Contract.Repositories;
using AzureFunction.Models;
using AzureFunction.UnitOfWork.DbContextAZ;

namespace AzureFunction.UnitOfWork.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContextServeur dbContext) : base(dbContext)
        {

        }
    }
}
