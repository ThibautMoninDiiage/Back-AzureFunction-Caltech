using SecurityServer.Contract.Repositories;
using SecurityServer.DataAccess.SecurityServerContext;
using SecurityServer.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServer.DataAccess.Repositories
{
    public class ApplicationUserRoleRepository : GenericRepositoryCustom<ApplicationUserRole>, IApplicationUserRoleRepository
    {
        public ApplicationUserRoleRepository(DbContextServer context) : base(context)
        {

        }
    }
}
