using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionEFCore
{
    public class Roles
    {
        public const string Route = "roles";

        [FunctionName("Roles")]
        public async Task<IActionResult> GetAllRoles(
            [HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = Route)] HttpRequest req,ILogger log)
        {
            log.LogInformation("Getting todo list items");

            var result = "coucou";

            return new OkObjectResult(result);
        }
    }
}
