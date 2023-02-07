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
using System.Linq;

namespace SecurityServer.Function
{
    public class Users
    {
        #region Public Variables
        public const string Route = "users";
        #endregion

        #region Private Variables
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        #endregion

        #region CTOR
        public Users(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }
        #endregion

        #region ServeurConnexion
        [FunctionName("ServeurConnexion")]
        [OpenApiOperation(operationId: "ServeurConnexion", tags: new[] { "User" })]
        [OpenApiRequestBody("userDtoUp", typeof(UserDtoUp))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GrantDtoDown), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> ServeurConnexion([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "signin")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserDtoUp userDtoUp = JsonConvert.DeserializeObject<UserDtoUp>(requestBody);

            if (userDtoUp.Mail == null || userDtoUp.Password == null)
                return new BadRequestResult();
            else
            {
                GrantDtoDown userResult = await _userService.Authenticate(userDtoUp);

                if (userResult == null)
                    return new BadRequestResult();
                else
                    return new OkObjectResult(userResult);
            }
        }
        #endregion

        #region ConnexionGrant
        [FunctionName("ConnexionGrant")]
        [OpenApiOperation(operationId: "ConnexionGrant", tags: new[] { "User" })]
        [OpenApiRequestBody("GrantDtoUp", typeof(GrantDtoUp))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDtoDown), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> ConnexionGrant([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authenticateGrant")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GrantDtoUp grantDtoUp  = JsonConvert.DeserializeObject<GrantDtoUp>(requestBody);

            if (grantDtoUp.CodeGrant == null || grantDtoUp.CodeGrant == "")
                return new BadRequestResult();
            else
            {
                UserDtoDown userResult = await _userService.GetToken(grantDtoUp.CodeGrant);

                if (userResult == null)
                    return new BadRequestResult();
                else
                    return new OkObjectResult(userResult);
            }
        }
        #endregion

        #region GetUserById
        [FunctionName("GetUserById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserGetByIdDtoDown), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> GetUserById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/{id}")]HttpRequest req,ILogger log, int? id)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                if (id == null)
                    return new BadRequestResult();
                else
                {
                    UserGetByIdDtoDown result = await _userService.GetById(id);

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
        #endregion

        #region CreateUser
        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "User" })]
        [OpenApiRequestBody("user",typeof(UserCreationDtoUp))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDtoDown), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "signup")] HttpRequest req, ILogger logger)
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
        #endregion

        #region ModifyUser
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
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

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
        #endregion

        #region AjoutExistantUser
        [FunctionName("AjoutExistantUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiRequestBody("AddUserInApplicationDtoDown", typeof(AddUserInApplicationDtoDown))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> AjoutExistantUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route+"/AddUser")] HttpRequest req, ILogger log)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                AddUserInApplicationDtoDown user = JsonConvert.DeserializeObject<AddUserInApplicationDtoDown>(requestBody);

                bool result = await _userService.AddExistantUser(user);

                if (!result)
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
        #endregion

        #region Delete
        [FunctionName("DeleteUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiParameter("id", In = ParameterLocation.Path, Description = "Id of the user to delete", Required = true, Type = typeof(int))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "Successfully deleted, or there was no User with the specified ID.")]
        public async Task<IActionResult> DeleteUser([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/{id}")] HttpRequest req, ILogger log, int id)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                bool result = await _userService.DeleteUser(id);

                return new OkObjectResult(result);
            }
            catch (AggregateException ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestResult();
            }
        }
        #endregion
    }
}
