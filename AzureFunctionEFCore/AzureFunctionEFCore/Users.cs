using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunction.Service.Interfaces;

namespace AzureFunctionEFCore
{
    public class Users
    {
        public const string Route = "users";
        private readonly IUserService _userService;

        public Users(IUserService userService)
        {
            _userService = userService;
        }

        [FunctionName("Users")]
        public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = Route)] HttpRequest req,ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string login = req.Query["login"];
            string mdp = req.Query["password"];

            if(login == null || mdp == null)
            {
                return new BadRequestResult();
            }

            var result = await _userService.Get(login,mdp);


            return new OkObjectResult(result);
        }
    }
}
