using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SecurityServer.Service.Interfaces;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;

namespace SecurityServer.Function
{
    public class Users
    {
        public const string Route = "users";
        private readonly IUserService _userService;

        public Users(IUserService userService)
        {
            _userService = userService;
        }

        [FunctionName("ServeurConnexion")]
        public async Task<IActionResult> ServeurConnexion([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = Route)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserDtoUp userDtoUp = JsonConvert.DeserializeObject<UserDtoUp>(requestBody);

            if (userDtoUp.UserName == null || userDtoUp.Password == null)
                return new BadRequestResult();
            else
            {
                UserDtoDown userResult = await _userService.Authenticate(userDtoUp);

                if (userResult == null)
                    return new EmptyResult();
                else
                    return new OkObjectResult(userResult);
            }
        }

        [FunctionName("GetUserById")]
        public async Task<IActionResult> GetUserById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/{id}")]HttpRequest req,ILogger log, int? id)
        {
            try
            {
                if (id == null)
                    return new BadRequestResult();
                else
                {
                    User result = await _userService.GetById(id);

                    if (result == null)
                        return new EmptyResult();
                    else
                        return new OkObjectResult(result);
                }
            }
            catch (AggregateException ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestResult();
            }
        }

        [FunctionName("CreateUser")]
        public async Task<IActionResult> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                User user = JsonConvert.DeserializeObject<User>(requestBody);

                User result = await _userService.CreateUser(user);

                if (result == null)
                    return new BadRequestResult();
                else
                    return new OkObjectResult(result);
            }
            catch (AggregateException ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestResult();
            }
        }
    }
}
