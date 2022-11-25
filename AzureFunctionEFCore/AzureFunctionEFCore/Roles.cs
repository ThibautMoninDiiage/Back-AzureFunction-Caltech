using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SecurityServer.Service.Interfaces;
using System.Collections.Generic;
using SecurityServer.Models.Models;

namespace SecurityServer.Function
{
    public class Roles
    {
        public const string Route = "roles";
        private readonly IRoleService _roleService;

        public Roles(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [FunctionName("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)])
        {
            IEnumerable<Role> result = await _roleService.GetAll();

            return new OkObjectResult(result);
        }
    }
}
