using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Http;
using TaskManager.Models;
using DAL;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskManagerService
{
    public static class AdminController
    {

        [FunctionName("register")]
        public static async Task<IActionResult> Register(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            User user = JsonConvert.DeserializeObject<User>(requestBody);

            string responseMessage = string.IsNullOrEmpty(user.Email)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {user.Email}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();
            UserManager userManager=new UserManager();
            User userfromdb = userManager.Get(user.Email);
            if (userfromdb != null)
                return new OkObjectResult("user with this email already exists");

            user.PasswordHash = SecurePasswordHasher.Hash(user.PasswordHash);
            user.Roles = new List<string> { "basic" };
            try
            {
                userManager.Insert(user);
                return new OkObjectResult("User registered");
            }
            catch (Exception)
            {
                return new InternalServerErrorResult();
            }
        }

        [FunctionName("Login")]
        public static async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            User login = JsonConvert.DeserializeObject<User>(requestBody);

            string responseMessage = string.IsNullOrEmpty(login.Email)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {login.Email}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();
            var user = new UserManager().Get(login.Email);
            if (user == null)
                return new OkObjectResult("Invalid credentials");

            var hashedPassword = SecurePasswordHasher.Hash(login.PasswordHash);

            var passwordhash = SecurePasswordHasher.Verify(login.PasswordHash, hashedPassword);
            if (passwordhash)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = new OkObjectResult(new Token()
                {
                    token = tokenString,
                    Username = login.Email,
                    Roles = user.Roles,
                    UserId = user.Id.ToString()
                });
            }
            return response;
        }

        [FunctionName("changepassword")]
        public static async Task<IActionResult> ChangePassword(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            User login = JsonConvert.DeserializeObject<User>(requestBody);

            string responseMessage = string.IsNullOrEmpty(login.Email)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {login.Email}. This HTTP triggered function executed successfully.";

            IActionResult response = new UnauthorizedResult();
            var user = new UserManager().Get(login.Email);
            if (user == null)
                return new OkObjectResult("Invalid credentials");

            var hashedPassword = SecurePasswordHasher.Hash(login.PasswordHash);

            var passwordhash = SecurePasswordHasher.Verify(login.PasswordHash, hashedPassword);

            if (passwordhash)
            {
                user.PasswordHash = SecurePasswordHasher.Hash(login.NewPassword);
                new UserManager().Update(user,user.Id);
                response = new OkObjectResult("Password changed");
            }
            return response;
        }

        private static string GenerateJSONWebToken(User userinfo)
        {

            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("jwt.key")));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.Email),
                new Claim(JwtRegisteredClaimNames.Email, userinfo.Email),
                new Claim("roles", "admin")
            };

            var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("jwt.issuer"),
              Environment.GetEnvironmentVariable("jwt.audience"),
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
