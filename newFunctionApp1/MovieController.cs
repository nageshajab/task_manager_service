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
    public class MovieController
    {

        [FunctionName("movielist")]
        public async Task<IActionResult> movielist(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MovieSearch movieSearch = JsonConvert.DeserializeObject<MovieSearch>(requestBody);

            IActionResult response = new UnauthorizedResult();

            List<Movie> lstmovies;
            try
            {
                lstmovies = new MovieManager().ListMoviesByUserId(movieSearch);              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            MovieIndexViewModel indexViewModel = new()
            {
                ListOfMovies = lstmovies.ToList(),
                MovieSearch = movieSearch
            };
            return new OkObjectResult(indexViewModel);
        }

        [FunctionName("getmovie")]
        public async Task<IActionResult> getmovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            int movieid = JsonConvert.DeserializeObject<Movie>(requestbody).Id;

            Movie movieFromDb =new MovieManager().Get( movieid);
            return new OkObjectResult(movieFromDb);
        }

        [FunctionName("addmovie")]
        public async Task<IActionResult> addmovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Movie incomingmovie = JsonConvert.DeserializeObject<Movie>(requestBody);

            Movie movie1 = new()
            {
                Actors = incomingmovie.Actors,
                UserId = incomingmovie.UserId,
                Rating = incomingmovie.Rating,
                Name = incomingmovie.Name,
                Description = incomingmovie.Description,
                Genre = incomingmovie.Genre,
                Language = incomingmovie.Language,
                Tags= incomingmovie.Tags,                
            };
            new MovieManager().Insert(movie1);
            return new OkObjectResult(movie1);
        }

        [FunctionName("updatemovie")]
        public async Task<IActionResult> updatemovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Movie incomingmovie = JsonConvert.DeserializeObject<Movie>(requestBody);
            new MovieManager().Update(incomingmovie,incomingmovie.Id);
            return new OkResult();
        }

        [FunctionName("deletemovie")]
        public async Task<IActionResult> deletemovie([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            int id = JsonConvert.DeserializeObject<Movie>(requestBody).Id;

            new MovieManager().DeleteMovie(id);
            
            return new OkResult();
        }

    }
}
