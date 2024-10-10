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
using System.Reflection.Metadata.Ecma335;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace FunctionApp1
{
    public class UrlController
    {
        private readonly MongoDbContext _context;

        public UrlController(MongoDbContext context)
        {
            _context = context;
        }

        [FunctionName("urltaglist")]
        public async Task<IActionResult> UrlTagList(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            UrlSearch urlSearch= JsonConvert.DeserializeObject<UrlSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            IQueryable<Url> lstUrls;
            List<Url> returnlist = new();
            List<string> tags = new();
            try
            {
                lstUrls= _context.Urls.Where(t => t.UserId == urlSearch.UserId);

                returnlist = lstUrls.ToList();

                foreach (var url in lstUrls)
                {
                    if (url.Tags == null)
                        continue;

                    foreach (string str in url.Tags)
                    {
                        if (!tags.Contains(str.Trim()))
                            tags.Add(str.Trim());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return new OkObjectResult(tags);
        }

        [FunctionName("urllist")]
        public async Task<IActionResult> UrlList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

           UrlSearch urlSearch     = JsonConvert.DeserializeObject<UrlSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            IQueryable<Url> lstUrls;
            List<Url> returnlist = new();
            List<string> tags = new();
            try
            {
                lstUrls= _context.Urls.Where(t => t.UserId == urlSearch.UserId);

                returnlist = lstUrls.ToList();

                foreach (var url in lstUrls)
                {
                    if (url.Tags == null)
                        continue;

                    foreach (string str in url.Tags)
                    {
                        if (!tags.Contains(str))
                            tags.Add(str);
                    }
                }

                //if filesearch contains any tag
                if (urlSearch.Tags != null && urlSearch.Tags.Length > 0)
                {
                    //iterate over files returned from database
                    foreach (var url in lstUrls)
                    {
                        //iterate over filesearch tags
                        foreach (string tag in urlSearch.Tags)
                        {
                            if (tag.Trim() == string.Empty)
                            {
                                continue;
                            }

                            if (!url.Tags.Contains(tag))
                            {
                                returnlist.Remove(url);
                            }
                        }
                    }
                }

                urlSearch.TotalRecords = returnlist.Count;

                //pagination at work
                returnlist = returnlist.Skip((urlSearch.PageNumber - 1) * 10).Take(10).ToList();

                if (urlSearch.SortBy != string.Empty)
                {
                    switch (urlSearch.SortBy.ToLower())
                    {
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            UrlIndexViewModel indexViewModel = new ()
            {
                ListOfUrls= returnlist,
                UrlSearch =urlSearch,
                Tags = tags
            };

            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("geturl")]
        public async Task<IActionResult> geturl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            
            string id = JsonConvert.DeserializeObject<Url1>(requestbody).Id.ToString();

            Url urlfromdb = _context.Urls.FirstOrDefault(t => t.Id.ToString() == id);
            return new OkObjectResult(urlfromdb);
        }

        private List<string> RemoveEmptyTags(Url url)
        {
            //remove emtpy tag from array
            List<string> tags = new List<string>();
            for (int i = 0; i < url.Tags.Length; i++)
            {
                if (url.Tags[i].Trim().Length != 0)
                {
                    tags.Add(url.Tags[i].Trim());
                }
            }
            return tags;
        }

        [FunctionName("addurl")]
        public async Task<IActionResult> AddUrl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Url url = JsonConvert.DeserializeObject<Url>(requestBody);
                        
            url.Tags=RemoveEmptyTags(url).ToArray();

            _context.Urls.Add(url);
            _context.SaveChanges();

            return new OkObjectResult(url);
        }

        [FunctionName("updateurl")]
        public async Task<IActionResult> UpdateUrl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Url1 url1  = JsonConvert.DeserializeObject<Url1>(requestBody);
            Url urlfromdb = _context.Urls.FirstOrDefault(t => t.Id.ToString() == url1.Id.ToString());

            if (urlfromdb== null)
            {
                return new NotFoundResult();
            }
            

            urlfromdb.Actress= url1.Actress;
            urlfromdb.UserId = url1.UserId;
            urlfromdb.Tags= url1.Tags;
            urlfromdb.Tags = RemoveEmptyTags(urlfromdb).ToArray();
            urlfromdb.Link= url1.Link;
            urlfromdb.Description = url1.Description;
            
            _context.SaveChanges();
            return new OkObjectResult(urlfromdb);
        }

        [FunctionName("deleteurl")]
        public async Task<IActionResult> DeleteUrl([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string id = JsonConvert.DeserializeObject<Url1>(requestBody).Id.ToString();

            Url urlfromdb = _context.Urls.FirstOrDefault(t => t.Id.ToString() == id.ToString());

            if (urlfromdb == null)
            {
                return new NotFoundResult();
            }

            _context.Urls.Remove(urlfromdb);
            _context.SaveChanges();

            return new OkObjectResult(urlfromdb);
        }

    }
}
