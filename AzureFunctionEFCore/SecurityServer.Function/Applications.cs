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
        public const string Route = "application";
        private readonly IApplicationService _applicationService;

        public Applications(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        #region GET
        [FunctionName("GetAllApplications")]                                                            //Get ALL
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ApplicationDtoDown>), Description = "All applications from the database.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent, Description = "The database contains no applications.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = "Something went wrong.")]
        public async Task<IActionResult> AcquireAllApplications([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
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

        [FunctionName("GetApplicationById")]                                                            //Get by ID
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

        #region POST
        [FunctionName("CreateApplication")]                                         // Create application
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiRequestBody(contentType: "application/json", typeof(ApplicationCreationDtoUp), Description = "The application to be created.", Required = true)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "Successfully created.")]
        public async Task<IActionResult> CreateApplication([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
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

        #region PUT
        [FunctionName("UpdateApplication")]                                                                 //Update application
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiRequestBody(contentType: "application/json", typeof(ApplicationUpdateDtoUp), Description = "The updated application. Comparison is ID-based.", Required = true)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "Successfully edited.")]
        public async Task<IActionResult> UpdateApplication([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route)] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ApplicationUpdateDtoUp updated = JsonConvert.DeserializeObject<ApplicationUpdateDtoUp>(requestBody);

                if (updated.Id == null)
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

        #region DELETE
        [FunctionName("DeleteApplication")]                                                                 //Delete application
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiParameter("id",In = ParameterLocation.Path ,Description = "Id of the application to delete",Required = true,Type = typeof(int))]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Successfully deleted, or there was no Application with the specified ID.")]
        public async Task<IActionResult> DeleteApplication([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/{id}")] HttpRequest req, ILogger log, int id)
        {
            try
            {
                string result = await _applicationService.DeleteApplication(id);

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
