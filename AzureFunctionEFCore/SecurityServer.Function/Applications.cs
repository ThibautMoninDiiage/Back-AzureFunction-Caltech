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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SecurityServer.Function
{
    public class Applications
    {
        #region Public Variables
        public const string Route = "application";
        #endregion

        #region Private Variables
        private readonly IApplicationService _applicationService;
        private readonly IAuthenticationService _authenticationService;
        #endregion

        #region CTOR
        public Applications(IApplicationService applicationService, IAuthenticationService authenticationService)
        {
            _applicationService = applicationService;
            _authenticationService = authenticationService;
        }
        #endregion

        #region GetAllApplications
        [FunctionName("GetAllApplications")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ApplicationDtoDown>), Description = "All applications from the database.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The database contains no applications.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Something went wrong.")]
        public async Task<IActionResult> AcquireAllApplications([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                var result = await _applicationService.GetAllApplications();

                if (result == null || result.Count() == 0)
                    return new NoContentResult();
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

        #region GetApplicationById
        [FunctionName("GetApplicationById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "The application from the database.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "No application with that ID was found.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "No ID was specified, or something went wrong.")]
        public async Task<IActionResult> GetApplicationById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/{id}")] HttpRequest req, ILogger log, int? id)
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
                    Application result = await _applicationService.GetById((int)id);

                    if (result == null)
                        return new NoContentResult();
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

        #region CreateApplication
        [FunctionName("CreateApplication")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiRequestBody(contentType: "application/json", typeof(ApplicationCreationDtoUp), Description = "The application to be created.", Required = true)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "Successfully created.")]
        public async Task<IActionResult> CreateApplication([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ApplicationCreationDtoUp created = JsonConvert.DeserializeObject<ApplicationCreationDtoUp>(requestBody);

                Application result = await _applicationService.CreateApplication(created);

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

        #region UpdateApplication
        [FunctionName("UpdateApplication")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiRequestBody(contentType: "application/json", typeof(ApplicationUpdateDtoUp), Description = "The updated application. Comparison is ID-based.", Required = true)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "Successfully edited.")]
        public async Task<IActionResult> UpdateApplication([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ApplicationUpdateDtoUp updated = JsonConvert.DeserializeObject<ApplicationUpdateDtoUp>(requestBody);

                if (updated == null)
                    return new BadRequestResult();

                Application result = await _applicationService.UpdateApplication(updated);

                return new OkObjectResult(result);
            }
            catch (AggregateException ex)
            {
                log.LogInformation(ex.Message);
                return new BadRequestResult();
            }
        }
        #endregion

        #region DeleteApplication
        [FunctionName("DeleteApplication")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiParameter("id",In = ParameterLocation.Path ,Description = "Id of the application to delete",Required = true,Type = typeof(int))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "Successfully deleted, or there was no Application with the specified ID.")]
        public async Task<IActionResult> DeleteApplication([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/{id}")] HttpRequest req, ILogger log, int id)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                if (id == 1)
                    return new BadRequestResult();
                else
                {
                    bool result = await _applicationService.DeleteApplication(id);

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

        #region GetUserWhereIsNotInAppli
        [FunctionName("GetUserWhereIsNotInAppli")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiParameter("id", In = ParameterLocation.Path, Description = "Id of the application", Required = true, Type = typeof(int))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ApplicationUserDtoDown>), Description = "GetAllUSersOk")]
        public async Task<IActionResult> GetUserWhereIsNotInAppli([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/users/{id}")] HttpRequest req, ILogger log, int id)
        {
            try
            {
                bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

                if (!verifyToken)
                {
                    return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
                }

                List<ApplicationUserDtoDown> result = await _applicationService.GetUserWhereIsNotInAppli(id);

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
