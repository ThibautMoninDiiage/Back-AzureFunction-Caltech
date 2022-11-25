using SecurityServer.Contract;
using SecurityServer.Contract.Repositories;
using SecurityServer.DataAccess;
using SecurityServer.DataAccess.SecurityServerContext;
using SecurityServer.Models.Models;

namespace SecurityServer.DataAccess.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(DbContextServeur dbContext) : base(dbContext)
        {

        }
    }
}
