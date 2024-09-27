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
    public class BlogController
    {
        private readonly MongoDbContext _context;

        public BlogController(MongoDbContext context)
        {
            _context = context;
        }

        [FunctionName("blogpostlist")]
        public  async Task<IActionResult> BlogList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            BlogPostSearch  blogPostSearch= JsonConvert.DeserializeObject<BlogPostSearch> (requestBody);

            IActionResult response =new  UnauthorizedResult();

            IQueryable<BlogPost> lstBlogPosts;
            try
            {
                lstBlogPosts= _context.BlogPosts.Where(t => t.UserId == blogPostSearch.UserId);

                blogPostSearch.TotalRecords = lstBlogPosts.Count();
                //pagination at work
                lstBlogPosts = lstBlogPosts.Skip((blogPostSearch.PageNumber - 1) * 10).Take(10);

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
            BlogPostIndexViewModel indexViewModel = new ()
            {
                ListOfBlogPosts = lstBlogPosts.ToList(),
                BlogPostSearch=blogPostSearch
            };
            return new OkObjectResult( indexViewModel);
        }

        [FunctionName("getBlogPost")]
        public async Task<IActionResult> getBlogPost([HttpTrigger(AuthorizationLevel.Anonymous,"post",Route =null)]HttpRequest req,ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            string taskid = JsonConvert.DeserializeObject<BlogPost1>(requestbody).Id.ToString();
            BlogPost blogpostFromDb = _context.BlogPosts.FirstOrDefault(t => t.Id.ToString() == taskid);
            return new OkObjectResult(blogpostFromDb);
        }

        [FunctionName("addBlogPost")]
        public async Task<IActionResult> AddBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TaskManager.Models.BlogPost blogPost= JsonConvert.DeserializeObject<TaskManager.Models.BlogPost>(requestBody);
            _context.BlogPosts.Add(blogPost);
            _context.SaveChanges();
            return new OkObjectResult(blogPost);
        }

        [FunctionName("updateBlogPost")]
        public async Task<IActionResult> UpdateBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            BlogPost1 blogPost1= JsonConvert.DeserializeObject<BlogPost1>(requestBody);
            BlogPost blogpostFromDb = _context.BlogPosts.FirstOrDefault(t => t.Id.ToString() == blogPost1.Id.ToString());
            if (blogpostFromDb == null)
            {
                return new NotFoundResult();
            }
            blogpostFromDb.Title = blogPost1.Title;
            blogpostFromDb.Description = blogPost1.Description;
            blogpostFromDb.Tags = blogPost1.Tags;
            blogpostFromDb.UserId = blogPost1.UserId;
            blogpostFromDb.RepositoryUrls = blogPost1.RepositoryUrls;
            blogpostFromDb.BlogPostUrl = blogpostFromDb.BlogPostUrl;

            _context.SaveChanges();
            return new OkObjectResult(blogpostFromDb);
        }

        [FunctionName("deleteblogpost")]
        public async Task<IActionResult> DeleteBlogPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string blogpostid = JsonConvert.DeserializeObject<BlogPost1>(requestBody).Id.ToString();

            TaskManager.Models.BlogPost blogpostFromDb = _context.BlogPosts.FirstOrDefault(t => t.Id.ToString() == blogpostid.ToString());

            if (blogpostFromDb == null)
            {
                return new NotFoundResult();
            }
            _context.BlogPosts.Remove(blogpostFromDb);
            _context.SaveChanges();
            return new OkObjectResult(blogpostFromDb);
        }

    }
}
