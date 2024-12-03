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
using DAL;
using System.Collections.Generic;

namespace FunctionApp1
{
    public class SugarReadingController
    {
        [FunctionName("sugarreadinglist")]
        public async Task<IActionResult> SugarReadingList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SugarReadingSearch sugarReadingsearch = JsonConvert.DeserializeObject<SugarReadingSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            List<SugarReading> sugarReadings;
            try
            {
                sugarReadings =new SugarReadingManager().List( sugarReadingsearch.UserId);

               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            SugarReadingIndexViewModel indexViewModel = new SugarReadingIndexViewModel()
            {
                ListOfSugarReadings = sugarReadings.ToList(),
                SugarReadingSearch = sugarReadingsearch
            };
            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("getsugarreading")]
        public async Task<IActionResult> getSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();

            int id = JsonConvert.DeserializeObject<SugarReading>(requestbody).Id;

            SugarReading sugarReadingfromdb =new SugarReadingManager().Get( id);
            return new OkObjectResult(sugarReadingfromdb);
        }

        [FunctionName("addsugarreading")]
        public async Task<IActionResult> AddSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SugarReading sugarReading = JsonConvert.DeserializeObject<SugarReading>(requestBody);
          new SugarReadingManager().Insert(sugarReading);
            return new OkObjectResult(sugarReading);
        }

        [FunctionName("updatesugarreading")]
        public async Task<IActionResult> UpdateSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SugarReading sugarReading1 = JsonConvert.DeserializeObject<SugarReading>(requestBody);
            new SugarReadingManager().Update(sugarReading1,sugarReading1.Id);
                       
            return new OkResult();
        }

        [FunctionName("deletesugarreading")]
        public async Task<IActionResult> DeleteSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<SugarReading>(requestBody).Id;

            new SugarReadingManager().DeleteSugarReadingByid(id);

            return new OkResult();
        }
    }
}
