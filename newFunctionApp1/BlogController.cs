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
    public class BlogController
    {
        [FunctionName("blogpostlist")]
        public async Task<IActionResult> BlogList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            BlogPostSearch blogPostSearch = JsonConvert.DeserializeObject<BlogPostSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            List<BlogPost> lstBlogPosts;
            try
            {
                DAL.BlogPostManager blogPostManager = new DAL.BlogPostManager();

                lstBlogPosts = blogPostManager.List(blogPostSearch.UserId);

                blogPostSearch.TotalRecords = lstBlogPosts.Count();
                //pagination at work
                lstBlogPosts = (List<BlogPost>)lstBlogPosts.Skip((blogPostSearch.PageNumber - 1) * 10).Take(10);

                if (blogPostSearch.SortBy != string.Empty)
                {
                    switch (blogPostSearch.SortBy.ToLower())
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
            BlogPostIndexViewModel indexViewModel = new()
            {
                ListOfBlogPosts = lstBlogPosts.ToList(),
                BlogPostSearch = blogPostSearch
            };
            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("getBlogPost")]
        public async Task<IActionResult> getBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = int.Parse(JsonConvert.DeserializeObject<BlogPost1>(requestbody).Id.ToString());
            BlogPost blogpostFromDb = new BlogPostManager().Get(id);
            return new OkObjectResult(blogpostFromDb);
        }

        [FunctionName("addBlogPost")]
        public async Task<IActionResult> AddBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            new BlogPostManager().Insert(JsonConvert.DeserializeObject<BlogPost>(requestBody));
            return new OkObjectResult(JsonConvert.DeserializeObject<BlogPost>(requestBody));
        }

        [FunctionName("updateBlogPost")]
        public async Task<IActionResult> UpdateBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            BlogPost blogPost1 = JsonConvert.DeserializeObject<BlogPost>(requestBody);
            var result = new BlogPostManager().Update(blogPost1, blogPost1.Id);
            return new OkObjectResult(result);
        }

        [FunctionName("deleteblogpost")]
        public async Task<IActionResult> DeleteBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int blogpostid = JsonConvert.DeserializeObject<BlogPost1>(requestBody).Id;

            BlogPostManager blogPostManager = new();
            var blogpostFromDb = blogPostManager.DeleteBlogPost(blogpostid);

            return new OkObjectResult(blogpostFromDb);
        }
    }
}
