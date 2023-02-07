using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;

namespace SecurityServer.Function
{
    public class Admins
    {
        #region Public Variables
        public const string Route = "panelAdmin";
        #endregion

        #region Private Variables
        private readonly IAdminService _adminService;
        private readonly IAuthenticationService _authenticationService;
        #endregion

        #region CTOR
        public Admins(IAdminService adminService, IAuthenticationService authenticationService)
        {
            _adminService = adminService;
            _authenticationService = authenticationService;
        }
        #endregion

        #region CreateUserByAdmin
        [FunctionName("CreateUserByAdmin")]
        [OpenApiOperation(operationId: "CreateUserByAdmin", tags: new[] { "Admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("userDtoUp", typeof(UserCreationDtoUp))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserAdminDtoDown), Description = "Response")]
        public async Task<IActionResult> CreateUserByAdmin([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = Route)] HttpRequest req, ILogger logger)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

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
        #endregion

        #region GetAllUsers
        [FunctionName("GetAllUsers")]
        [OpenApiOperation(operationId: "GetAllUsers", tags: new[] { "Admin" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<UserAllDtoDown>), Description = "Response")]
        public async Task<IActionResult> GetAllUsers([HttpTrigger(AuthorizationLevel.Anonymous,"get",Route = Route)]HttpRequest req,ILogger logger)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

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
        #endregion
    }
}

