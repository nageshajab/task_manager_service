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

namespace FunctionApp1
{
    public class AlexaController
    {
        private readonly MongoDbContext _context;

        public AlexaController(MongoDbContext context)
        {
            _context = context;
        }


        [FunctionName("therapyvisitRemaining")]
        public async Task<IActionResult> rolelist(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            IActionResult response = new UnauthorizedResult();

            var patient = _context.Patients.FirstOrDefault();

            string responseMessage = $"you have {patient.therapyVisitsRemaining} thearapy visits remaining, out of maximum benifit of {patient.totalTherapyVisits}";
            response = new OkObjectResult(responseMessage);

            return response;
        }

    }
}
