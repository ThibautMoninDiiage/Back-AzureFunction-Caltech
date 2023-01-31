using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;

namespace SecurityServer.Function
{
    public class Admins
    {
        public const string Route = "panelAdmin";
        private readonly IAdminService _adminService;

        public Admins(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [FunctionName("CreateUserByAdmin")]
        [OpenApiOperation(operationId: "CreateUserByAdmin", tags: new[] { "Admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("userDtoUp", typeof(UserCreationDtoUp))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserAdminDtoDown), Description = "Response")]
        public async Task<IActionResult> CreateUserByAdmin([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = Route)] HttpRequest req, ILogger logger)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserCreationDtoUp user = JsonConvert.DeserializeObject<UserCreationDtoUp>(requestBody);

                UserAdminDtoDown result = await _adminService.CreateUser(user);

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

        [FunctionName("GetAllUsers")]
        [OpenApiOperation(operationId: "GetAllUsers", tags: new[] { "Admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<UserAllDtoDown>), Description = "Response")]
        public async Task<IActionResult> GetAllUsers([HttpTrigger(AuthorizationLevel.Anonymous,"get",Route = Route)]HttpRequest req,ILogger logger)
        {
            try
            {
                List<UserAllDtoDown> result = await _adminService.GetAllUsers();

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
    }
}

