using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Description = "The database contains no applications.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void), Description = "Something went wrong.")]
        public async Task<IActionResult> AcquireAllApplications([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route + "/All")] HttpRequest req, ILogger log)
        {
            try
            {
                var result = await _applicationService.GetAllApplications();

                if (result == null)
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
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "The application from the database.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Description = "No application with that ID was found.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void), Description = "No ID was specified, or something went wrong.")]
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
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "Successfully created.")]
        public async Task<IActionResult> CreateApplication([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route + "/create")] HttpRequest req, ILogger log)
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

        [FunctionName("UpdateApplication")]                                                                 //Update application
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Application), Description = "Successfully edited.")]
        public async Task<IActionResult> UpdateApplication([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route + "/update")] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Application updated = JsonConvert.DeserializeObject<Application>(requestBody);

                Application result = await _applicationService.UpdateApplication(updated);

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

        [FunctionName("DeleteApplication")]                                                                 //Delete application
        [OpenApiOperation(operationId: "Run", tags: new[] { "Application" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ApplicationDeleteDtoUp), Description = "Successfully deleted, or there was no Application with the specified ID.")]
        public async Task<IActionResult> DeleteApplication([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route + "/delete")] HttpRequest req, ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ApplicationDeleteDtoUp removed = JsonConvert.DeserializeObject<ApplicationDeleteDtoUp>(requestBody);

                string result = await _applicationService.DeleteApplication(removed.Id);

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
