using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using SecurityServer.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SecurityServer.Service.DTO.Down;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Linq;

namespace SecurityServer.Function
{
    public class Roles
    {
        #region Public Variables
        public const string Route = "roles";
        #endregion

        #region Private Variables
        private readonly IRoleService _roleService;
        private readonly IAuthenticationService _authenticationService;
        #endregion

        #region CTOR
        public Roles(IRoleService roleService, IAuthenticationService authenticationService)
        {
            _roleService = roleService;
            _authenticationService = authenticationService;
        }
        #endregion

        #region GetAllRoles
        [FunctionName("GetAllRoles")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Role" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<RoleDtoDown>), Description = "Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(void))]
        public async Task<IActionResult> GetAllRoles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)]HttpRequest req,ILogger logger)
        {
            bool verifyToken = _authenticationService.VerifyToken(req.Headers["Bearer"].FirstOrDefault());

            if (!verifyToken)
            {
                return new ContentResult() { Content = "Erreur token non valide ou absent", StatusCode = (int)HttpStatusCode.Unauthorized };
            }

            IEnumerable<RoleDtoDown> result = await _roleService.GetAll();

            if (result != null)
                return new OkObjectResult(result);
            else
                return new BadRequestResult();
        }
        #endregion
    }
}
