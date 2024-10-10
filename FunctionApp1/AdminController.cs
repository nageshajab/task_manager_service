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
using ZstdSharp.Unsafe;

namespace FunctionApp1
{
    public class AdminController
    {
        private readonly MongoDbContext _context;

        public AdminController(MongoDbContext context)
        {
            _context = context;
        }


        [FunctionName("Login")]
        public  async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            User login = JsonConvert.DeserializeObject<User> (requestBody);

            string responseMessage = string.IsNullOrEmpty(login.Email)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {login.Email}. This HTTP triggered function executed successfully.";

            IActionResult response =new  UnauthorizedResult();
                     
            var user = _context.Users.Where(u => u.Email == login.Email).FirstOrDefault();
            if (user == null)
                return new OkObjectResult("Invalid credentials");

            var hashedPassword = SecurePasswordHasher.Hash(login.PasswordHash);

            var passwordhash = SecurePasswordHasher.Verify(login.PasswordHash, hashedPassword);
            if (passwordhash)
            {
                var tokenString = GenerateJSONWebToken(user);
                TaskManager.Models.Token token = new TaskManager.Models.Token()
                {
                    token = tokenString,
                    Username = login.Email,
                    Roles = user.Roles,
                    UserId = user.Id.ToString()
                };
                response =new OkObjectResult(token);
            }
            return response;
        }

        [FunctionName("changepassword")]
        public async Task<IActionResult> ChangePassword(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            User login = JsonConvert.DeserializeObject<User>(requestBody);

            string responseMessage = string.IsNullOrEmpty(login.Email)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {login.Email}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();

            var user = _context.Users.Where(u => u.Email == login.Email).FirstOrDefault();
            if (user == null)
                return new OkObjectResult("Invalid credentials");

            var hashedPassword = SecurePasswordHasher.Hash(login.PasswordHash);

            var passwordhash = SecurePasswordHasher.Verify(login.PasswordHash, hashedPassword);

            if (passwordhash)
            {
                user.PasswordHash = SecurePasswordHasher.Hash(login.NewPassword);
                _context.Users.Update(user);
                _context.SaveChanges();
              
                response = new OkObjectResult("Password changed");
            }
            return response;
        }

        private static string GenerateJSONWebToken(User userInfo)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt.Key")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim("roles", "admin")
            };

            var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("Jwt.Issuer"),
              Environment.GetEnvironmentVariable("Jwt.Audience"),
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [FunctionName("register")]
        public async Task<IActionResult> Register(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            User user = JsonConvert.DeserializeObject<User>(requestBody);

            string responseMessage = string.IsNullOrEmpty(user.Email)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {user.Email}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();
            if (_context.Users.Where(u => u.Email == user.Email).Any())
                return new BadRequestResult();

            user.PasswordHash = SecurePasswordHasher.Hash(user.PasswordHash);
            user.Roles = new List<string> { "basic" };
            _context.Users.Add(user);
           
            int result = _context.SaveChanges();

            if (result > 0)
                return new OkObjectResult("User registered");
            else
                return new InternalServerErrorResult();
        }
    }
}
