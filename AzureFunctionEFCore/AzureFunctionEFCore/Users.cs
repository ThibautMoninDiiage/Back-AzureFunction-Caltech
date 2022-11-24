using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using AzureFunction.Service.Interfaces;
using Microsoft.Extensions.Logging;
using AzureFunction.Models.DTO.Up;

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

        [FunctionName("Connexion")]
        public async Task<IActionResult> Connexion([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = Route)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserDtoUp userDtoUp = JsonConvert.DeserializeObject<UserDtoUp>(requestBody);

            if (userDtoUp.UserName == null || userDtoUp.Password == null)
            {
                return new BadRequestResult();
            }

            var result = await _userService.Get(userDtoUp.UserName, userDtoUp.Password);

            if (result == null)
            {
                return new EmptyResult();
            }

            return new OkObjectResult(result);
        }

        [FunctionName("UserById")]
        public async Task<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)] HttpRequest req,ILogger log)
        {
            try
            {
                string idRecu = req.Query["id"];
                if (idRecu == null)
                {
                    return new BadRequestResult();
                }
                int id = int.Parse(idRecu);
                if (id == 0)
                {
                    return new BadRequestResult();
                }

                var result = await _userService.GetById(id);

                if (result == null)
                {
                    return new EmptyResult();
                }

                return new OkObjectResult(result);
            }
            catch(AggregateException ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestResult();
            }
        }
    }
}
