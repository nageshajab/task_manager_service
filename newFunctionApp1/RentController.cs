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
using System.Collections.Generic;
using Models;

namespace FunctionApp1
{
    public class RentController
    {
        private DAL.RentManager rentManager;

        [FunctionName("rentlist")]
        public async Task<IActionResult> RentList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RentSearch rentSearch = JsonConvert.DeserializeObject<RentSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            try
            {
                rentManager = new DAL.RentManager();
                rentSearch = rentManager.ListRentsByUserId(rentSearch);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            RentIndexViewModel indexViewModel = new()
            {
                Rents = rentSearch.Rents,
                RentSearch = rentSearch
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("tenantlist")]
        public async Task<IActionResult> TenantList(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TenantSearch tenantSearch= JsonConvert.DeserializeObject<TenantSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            try
            {
                rentManager = new DAL.RentManager();
                tenantSearch= rentManager.ListTenantsByUserId(tenantSearch);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            TenantIndexViewModel indexViewModel = new()
            {
                Tenants= tenantSearch.Tenants,
                TenantSearch = tenantSearch
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("gettenant")]
        public async Task<IActionResult> getTenant([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager = new DAL.RentManager();
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            int taskid = JsonConvert.DeserializeObject<TaskManager.Models.Task>(requestbody).Id;
            Tenant tenantFromDb = rentManager.Get(taskid);
            return new OkObjectResult(tenantFromDb);
        }

        [FunctionName("addtenant")]
        public async Task<IActionResult> AddTenant([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager = new DAL.RentManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Tenant tenant = JsonConvert.DeserializeObject<Tenant>(requestBody);

            rentManager.InsertTenant(tenant);

            return new OkResult();
        }

        [FunctionName("addrent")]
        public async Task<IActionResult> AddRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager = new DAL.RentManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rent rent = JsonConvert.DeserializeObject<Rent>(requestBody);

            rentManager.InsertRent(rent);

            return new OkResult();
        }

        [FunctionName("updatetenant")]
        public async Task<IActionResult> UpdateTenant([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager= new DAL.RentManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Tenant tenant = JsonConvert.DeserializeObject<Tenant>(requestBody);
            rentManager.UpdateTenant(tenant, tenant.Id);

            return new OkResult();
        }

        [FunctionName("updaterent")]
        public async Task<IActionResult> UpdateRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager = new DAL.RentManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rent rent= JsonConvert.DeserializeObject<Rent>(requestBody);
            rentManager.UpdateRent(rent, rent.Id);

            return new OkResult();
        }

        [FunctionName("deletetenant")]
        public async Task<IActionResult> DeleteTenant([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager= new DAL.RentManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<Tenant>(requestBody).Id;

            rentManager.DeleteTenant(id);

            return new OkResult();
        }

        [FunctionName("deleterent")]
        public async Task<IActionResult> DeleteRent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            rentManager = new DAL.RentManager();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<Rent>(requestBody).Id;

            rentManager.DeleteRent(id);

            return new OkResult();
        }
    }
}
