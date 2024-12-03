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
using DAL;

namespace FunctionApp1
{
    public class UserController
    {

        [FunctionName("userlist")]
        public async Task<IActionResult> userlist(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserSearch usersearch = JsonConvert.DeserializeObject<UserSearch>(requestBody);

            string responseMessage = string.IsNullOrEmpty(usersearch.Name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {usersearch.Name}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();

            var users =new UserManager().List();

            response = new OkObjectResult(users);

            return response;
        }

    }
}
