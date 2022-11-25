using SecurityServer.Contract.Repositories;
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
