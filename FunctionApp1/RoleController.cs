using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TaskManager.Models;
using System.Linq;
using TaskManagerService;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Http;
using System.Collections.Generic;

namespace FunctionApp1
{
    public class RoleController
    {
        private readonly MongoDbContext _context;

        public RoleController(MongoDbContext context)
        {
            _context = context;
        }


        [FunctionName("rolelist")]
        public async Task<IActionResult> rolelist(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RoleSearch rolesearch= JsonConvert.DeserializeObject<RoleSearch>(requestBody);

            string responseMessage = string.IsNullOrEmpty(rolesearch.Name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {rolesearch.Name}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();

            var roles = _context.Roles.ToList();

            response = new OkObjectResult(roles);

            return response;
        }

    }
}
