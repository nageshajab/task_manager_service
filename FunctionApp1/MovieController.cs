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
using System.Collections.Generic;

namespace FunctionApp1
{
    public class MovieController
    {
        private readonly MongoDbContext _context;

        public MovieController(MongoDbContext context)
        {
            _context = context;
        }

        [FunctionName("movielist")]
        public  async Task<IActionResult> movielist(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MovieSearch movieSearch= JsonConvert.DeserializeObject<MovieSearch> (requestBody);

            IActionResult response =new  UnauthorizedResult();

            IQueryable<TaskManager.Models.Movie> lstmovies;
            try
            {
                lstmovies= _context.Movies.Where(t => t.UserId == movieSearch.UserId);
                movieSearch.TotalRecords = lstmovies.Count();
                //pagination at work
                lstmovies= lstmovies.Skip((movieSearch.PageNumber - 1) * 10).Take(10);

                if (movieSearch.SortBy != string.Empty)
                {
                    switch (movieSearch.SortBy.ToLower())
                    {
                        case "genre":
                            lstmovies = lstmovies.OrderBy(t => t.Genre);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            MovieIndexViewModel indexViewModel = new ()
            {
                ListOfMovies= lstmovies.ToList(),
                MovieSearch= movieSearch
            };
            return new OkObjectResult( indexViewModel);
        }

        [FunctionName("getmovie")]
        public async Task<IActionResult> getmovie([HttpTrigger(AuthorizationLevel.Anonymous,"post",Route =null)]HttpRequest req,ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            string movieid = JsonConvert.DeserializeObject<Movie1>(requestbody).Id.ToString();
            Movie movieFromDb = _context.Movies.FirstOrDefault(t => t.Id.ToString() == movieid);
            return new OkObjectResult(movieFromDb);
        }

        [FunctionName("addmovie")]
        public async Task<IActionResult> addmovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Movie1 incomingmovie= JsonConvert.DeserializeObject<Movie1>(requestBody);
            Movie movie1 = new ()
            {
                Actors = new List<string>(incomingmovie.Actors.Split(',')),
                UserId = incomingmovie.UserId,
                Rating = incomingmovie.Rating,
                Name = incomingmovie.Name,
                Description = incomingmovie.Description,
                Genre = incomingmovie.Genre,
                Language = incomingmovie.Language
            };
            _context.Movies.Add(movie1);
            _context.SaveChanges();
            return new OkObjectResult(movie1);
        }

        [FunctionName("updatemovie")]
        public async Task<IActionResult> updatemovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Movie1 incomingmovie= JsonConvert.DeserializeObject<Movie1>(requestBody);
            Movie movieFromDb = _context.Movies.FirstOrDefault(t => t.Id.ToString() == incomingmovie.Id.ToString());
            if (movieFromDb == null)
            {
                return new NotFoundResult();
            }
            movieFromDb.Actors=new List<string>( incomingmovie.Actors.Split(','));
            movieFromDb.UserId= incomingmovie.UserId;
            movieFromDb.Rating= incomingmovie.Rating;
            movieFromDb.Name= incomingmovie.Name;
            movieFromDb.Description= incomingmovie.Description;
            movieFromDb.Genre= incomingmovie.Genre;
            _context.SaveChanges();
            return new OkObjectResult(movieFromDb);
        }

        [FunctionName("deletemovie")]
        public async Task<IActionResult> deletemovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string movieid = JsonConvert.DeserializeObject<Movie1>(requestBody).Id.ToString();

            TaskManager.Models.Task taskFromDb = _context.Tasks.FirstOrDefault(t => t.Id.ToString() == movieid.ToString());
            if (taskFromDb == null)
            {
                return new NotFoundResult();
            }
            _context.Tasks.Remove(taskFromDb);
            _context.SaveChanges();
            return new OkObjectResult(taskFromDb);
        }

    }
}
