using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunction.Service.Interfaces;

namespace AzureFunctionEFCore
{
    public class Roles
    {
        public const string Route = "roles";
        public IRoleService _roleService { get; set; }

        public Roles(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [FunctionName("Roles")]
        public async Task<IActionResult> GetAllRoles(
            [HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = Route)] HttpRequest req,ILogger log)
        {
            log.LogInformation("Getting todo list items");

            var result = await _roleService.GetAll();

            return new OkObjectResult(result);
        }
    }
}
