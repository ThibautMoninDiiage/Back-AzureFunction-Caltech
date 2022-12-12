using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using SecurityServer.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SecurityServer.Service.DTO.Down;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

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
        [OpenApiOperation(operationId: "Run", tags: new[] { "Role" })]
        public async Task<IActionResult> GetAllRoles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)]HttpRequest req,ILogger logger)
        {
            IEnumerable<RoleDtoDown> result = await _roleService.GetAll();

            return new OkObjectResult(result);
        }
    }
}
