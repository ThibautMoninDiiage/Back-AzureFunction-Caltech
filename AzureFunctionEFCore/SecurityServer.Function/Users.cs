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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

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
        [OpenApiOperation(operationId: "ServeurConnexion", tags: new[] { "User" })]
        [OpenApiRequestBody("userDtoUp", typeof(UserDtoUp))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDtoDown), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> ServeurConnexion([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "signin")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserDtoUp userDtoUp = JsonConvert.DeserializeObject<UserDtoUp>(requestBody);

            if (userDtoUp.Mail == null || userDtoUp.Password == null)
                return new BadRequestResult();
            else
            {
                UserDtoDown userResult = await _userService.Authenticate(userDtoUp);

                if (userResult == null)
                    return new BadRequestResult();
                else
                    return new OkObjectResult(userResult);
            }
        }

        [FunctionName("GetUserById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
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
                        return new BadRequestResult();
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
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User" })]
        [OpenApiRequestBody("user",typeof(UserCreationDtoUp))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDtoDown), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "signup")] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserCreationDtoUp user = JsonConvert.DeserializeObject<UserCreationDtoUp>(requestBody);

                UserDtoDown result = await _userService.CreateUser(user);

                if (result == null)
                    return new BadRequestResult();
                else
                    return new OkObjectResult(result);
            }
            catch (AggregateException ex)
            {
                logger.LogInformation(ex.Message);
                return new BadRequestResult();
            }
        }

        [FunctionName("ModifyUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiRequestBody("user", typeof(UserModifyDtoUp))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> ModifyUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserModifyDtoUp user = JsonConvert.DeserializeObject<UserModifyDtoUp>(requestBody);

                User result = await _userService.UpdateUser(user);

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
