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
using DAL;

namespace FunctionApp1
{
    public class UrlController
    {

        [FunctionName("urllist")]
        public async Task<IActionResult> UrlList(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            UrlSearch urlSearch = JsonConvert.DeserializeObject<UrlSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            List<Url> lstUrls;

            try
            {
                lstUrls = new UrlManager().List(urlSearch.UserId);
            }
            catch (Exception)
            {
                throw;
            }

            return new OkObjectResult(lstUrls);
        }

        [FunctionName("geturl")]
        public async Task<IActionResult> geturl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();

            int id = JsonConvert.DeserializeObject<Url>(requestbody).Id;

            Url urlfromdb = new UrlManager().Get(id);
            return new OkObjectResult(urlfromdb);
        }

        [FunctionName("addurl")]
        public async Task<IActionResult> AddUrl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Url url = JsonConvert.DeserializeObject<Url>(requestBody);
            new UrlManager().Insert(url);

            return new OkObjectResult(url);
        }

        [FunctionName("updateurl")]
        public async Task<IActionResult> UpdateUrl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Url url1 = JsonConvert.DeserializeObject<Url>(requestBody);
            new UrlManager().Update(url1, url1.Id);
            return new OkResult();
        }

        [FunctionName("deleteurl")]
        public async Task<IActionResult> DeleteUrl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<Url>(requestBody).Id;

            new UrlManager().DeleteUrl(id);

            return new OkResult();
        }

    }
}
