using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctionEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureFunctionEFCore
{
    public class Roles
    {

        private readonly DbContextEFCore _dbContext;
        public const string Route = "roles";

        public Roles(DbContextEFCore dbContext)
        {
            this._dbContext = dbContext;
        }

        [FunctionName("Roles")]
        public async Task<IActionResult> GetAllRoles(
            [HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = Route)] HttpRequest req,ILogger log)
        {
            log.LogInformation("Getting todo list items");

            var result = await _dbContext.Roles.ToListAsync();

            return new OkObjectResult(result);
        }
    }
}
