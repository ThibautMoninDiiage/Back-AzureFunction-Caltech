using SecurityServer.Contract.Repositories;
using SecurityServer.DataAccess.SecurityServerContext;
using SecurityServer.Models.Models;

namespace SecurityServer.DataAccess.Repositories
{
    public class GrantRepository : GenericRepositoryCustom<Grant>, IGrantRepository
    {
        public GrantRepository(DbContextServer context) : base(context)
        {

        }
    }
}
