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

namespace FunctionApp1
{
    public class SugarReadingController
    {
        private readonly MongoDbContext _context;

        public SugarReadingController(MongoDbContext context)
        {
            _context = context;
        }

        [FunctionName("sugarreadinglist")]
        public  async Task<IActionResult> SugarReadingList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SugarReadingSearch sugarReadingsearch = JsonConvert.DeserializeObject<SugarReadingSearch> (requestBody);

            IActionResult response =new  UnauthorizedResult();

            IQueryable<SugarReading> sugarReadings;
            try
            {
                sugarReadings= _context.SugarReadings.Where(t => t.UserId == sugarReadingsearch.UserId);

                sugarReadingsearch.TotalRecords = sugarReadings.Count();

                //pagination at work
                sugarReadings= sugarReadings.Skip((sugarReadingsearch.PageNumber - 1) * 10).Take(10);

                if (sugarReadingsearch.SortBy != string.Empty)
                {
                    switch (sugarReadingsearch.SortBy.ToLower())
                    {
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            SugarReadingIndexViewModel indexViewModel = new SugarReadingIndexViewModel()
            {
                ListOfSugarReadings = sugarReadings.ToList(),
                SugarReadingSearch= sugarReadingsearch
            };
            return new OkObjectResult( indexViewModel);
        }

        [FunctionName("getsugarreading")]
        public async Task<IActionResult> getSugarReading([HttpTrigger(AuthorizationLevel.Anonymous,"post",Route =null)]HttpRequest req,ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            
            string id = JsonConvert.DeserializeObject<SugarReading1>(requestbody).Id.ToString();

            SugarReading  sugarReadingfromdb= _context.SugarReadings.FirstOrDefault(t => t.Id.ToString() == id);
            return new OkObjectResult(sugarReadingfromdb);
        }

        [FunctionName("addsugarreading")]
        public async Task<IActionResult> AddSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SugarReading sugarReading = JsonConvert.DeserializeObject<SugarReading>(requestBody);
            _context.SugarReadings.Add(sugarReading);
            _context.SaveChanges();
            return new OkObjectResult(sugarReading);
        }

        [FunctionName("updatesugarreading")]
        public async Task<IActionResult> UpdateSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SugarReading1 sugarReading1= JsonConvert.DeserializeObject<SugarReading1>(requestBody);
            SugarReading sugarreadingfromdb = _context.SugarReadings.FirstOrDefault(t => t.Id.ToString() == sugarReading1.Id.ToString());

            if (sugarreadingfromdb== null)
            {
                return new NotFoundResult();
            }

            sugarreadingfromdb.PP= sugarReading1.PP;
            sugarreadingfromdb.Fasting= sugarReading1.Fasting;
            sugarreadingfromdb.Medicines= sugarReading1.Medicines;
            sugarreadingfromdb.Date = sugarReading1.Date;
            sugarreadingfromdb.Weight= sugarReading1.Weight;
                        
            _context.SaveChanges();
            return new OkObjectResult(sugarreadingfromdb);
        }

        [FunctionName("deletesugarreading")]
        public async Task<IActionResult> DeleteSugarReading([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string id = JsonConvert.DeserializeObject<SugarReading1>(requestBody).Id.ToString();

            SugarReading sugarReadingfromdb = _context.SugarReadings.FirstOrDefault(t => t.Id.ToString() == id.ToString());

            if (sugarReadingfromdb== null)
            {
                return new NotFoundResult();
            }

            _context.SugarReadings.Remove(sugarReadingfromdb);
            _context.SaveChanges();

            return new OkObjectResult(sugarReadingfromdb);
        }

    }
}
