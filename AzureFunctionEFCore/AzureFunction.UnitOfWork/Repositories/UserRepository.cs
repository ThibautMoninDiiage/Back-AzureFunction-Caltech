using SecurityServer.Contract.Repositories;
using SecurityServer.DataAccess.DbContextAZ;
using SecurityServer.Models.Models;

namespace SecurityServer.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContextServeur dbContext) : base(dbContext)
        {

        }
    }
}
