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
    public class RentController
    {
        private readonly MongoDbContext _context;

        public RentController(MongoDbContext context)
        {
            _context = context;
        }

        [FunctionName("rentlist")]
        public async Task<IActionResult> RentList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RentSearch rentSearch = JsonConvert.DeserializeObject<RentSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            IQueryable<Rent> rents;
            try
            {
                rents = _context.Rents.Where(t => t.UserId == rentSearch.UserId);

                rentSearch.TotalRecords = rents.Count();
                //pagination at work
                rents = rents.Skip((rentSearch.PageNumber - 1) * 10).Take(10);

                if (rentSearch.SortBy != string.Empty)
                {
                    switch (rentSearch.SortBy.ToLower())
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
            RentIndexViewModel indexViewModel = new()
            {
                Rents = rents.ToList(),
                RentSearch = rentSearch
            };
            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("getRent")]
        public async Task<IActionResult> getRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            string rentid= JsonConvert.DeserializeObject<Rent1>(requestbody).Id.ToString();
            Rent rentfromdb= _context.Rents.FirstOrDefault(t => t.Id.ToString() == rentid);
            return new OkObjectResult(rentfromdb);
        }

        [FunctionName("addrent")]
        public async Task<IActionResult> AddRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rent rent= JsonConvert.DeserializeObject<TaskManager.Models.Rent>(requestBody);
            _context.Rents.Add(rent);
            _context.SaveChanges();
            return new OkObjectResult(rent);
        }

        [FunctionName("updaterent")]
        public async Task<IActionResult> UpdateRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rent1 rent1= JsonConvert.DeserializeObject<Rent1>(requestBody);
            Rent rentfromdb= _context.Rents.FirstOrDefault(t => t.Id.ToString() == rent1.Id.ToString());

            if (rentfromdb== null)
            {
                return new NotFoundResult();
            }

            rentfromdb.Tenant = rent1.Tenant;
            rentfromdb.Amount = rent1.Amount;
            rentfromdb.Date= rent1.Date;
          
            _context.SaveChanges();
            return new OkObjectResult(rentfromdb);
        }

        [FunctionName("deleterent")]
        public async Task<IActionResult> DeleteRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string rentid = JsonConvert.DeserializeObject<Rent1>(requestBody).Id.ToString();

            Rent rentfromdb= _context.Rents.FirstOrDefault(t => t.Id.ToString() == rentid.ToString());

            if (rentfromdb== null)
            {
                return new NotFoundResult();
            }

            _context.Rents.Remove(rentfromdb);
            _context.SaveChanges();
            return new OkObjectResult(rentfromdb);
        }
    }
}
