using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Web.Resource;
using System.Collections.Generic;
using System.Linq;
using TodoListService.Models;

namespace TodoListService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [RequiredScope(scopeRequiredByAPI)]
    public class UserController : Controller
    {
        const string scopeRequiredByAPI = "users.read";

        private readonly IHttpContextAccessor _contextAccessor;

        public UserController(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<User> Get()
        {
            List<User> users = new();

            SqlCommand sqlCommand = GetConnection().CreateCommand();
            sqlCommand.CommandText = "select * from users";

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User()
                {
                    ClientCode = reader["ClientCode"].ToString(),
                    DisplayName = reader["DisplayName"].ToString(),
                    Surname = reader["Surname"].ToString(),
                    GivenName = reader["GivenName"].ToString(),
                    email = reader["email"].ToString(),
                    TaxId = reader["TaxId"].ToString(),
                    ObjectId = reader["objectId"].ToString()
                });
            }

            return users;
        }

        public SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Server=tcp:azb2c.database.windows.net,1433;Initial Catalog=azb2c;Persist Security Info=False;User ID=nageshajab;Password=Password1@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            conn.Open();
            return conn;
        }

        // GET: api/values
        [HttpGet("{objectId}", Name = "GetUser")]
        public User Get(string ObjectId)
        {
            User user1 = new();
            SqlCommand sqlCommand = GetConnection().CreateCommand();
            sqlCommand.CommandText = $"select * from users where objectId='{ObjectId}'";

            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                user1 = new User()
                {
                    ClientCode = reader["ClientCode"].ToString(),
                    DisplayName = reader["DisplayName"].ToString(),
                    Surname = reader["Surname"].ToString(),
                    GivenName = reader["GivenName"].ToString(),
                    email = reader["email"].ToString(),
                    TaxId = reader["TaxId"].ToString(),
                    ObjectId = reader["objectId"].ToString()
                };
            }
            return user1;
        }

        [HttpDelete("{objectId}")]
        public IActionResult Delete(string objectId)
        {
            User user1 = new();
            SqlCommand sqlCommand = GetConnection().CreateCommand();
            sqlCommand.CommandText = $"delete users where objectId={objectId}";

            int rows = sqlCommand.ExecuteNonQuery();
            return Ok(rows);
        }

        // PATCH api/values
        [HttpPatch]
        public IActionResult Patch([FromBody] User user)
        {
            SqlCommand sqlCommand = GetConnection().CreateCommand();

            string commandText = $"update users "
                + $" set clientcode='{user.ClientCode}',"
                + $"taxid='{user.TaxId}'" +
                $" where objectId='{user.ObjectId}'";

            sqlCommand.CommandText = commandText;

            sqlCommand.ExecuteNonQuery();

            var user1=Get(user.ObjectId);
            return Ok(user1);
        }
    }
}