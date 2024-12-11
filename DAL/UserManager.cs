using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace DAL
{
    public class UserManager
    {
        public string paramId = "id";
        public string paramPasswordHash = "passwordHash";
        public string paramNewPassword = "newPassword";
        public string paramEmail = "email";
        public string paramRoles = "roles";
        public string paramUserId = "userid";
        public string paramRoleId = "roleid";

        public string querySelectUsers = "select userquery.*,rolequery.roles from (select u.email,STRING_AGG(name, ', ') as roles from roles r,userroles ur,users u where r.id=ur.RoleId and ur.Userid=u.id group by email) rolequery, (select u.email, u.id,u.passwordHash,u.newPassword from users u) userquery where rolequery.email=userquery.email";

        public List<User> List()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = querySelectUsers;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User User = new User();
                    User.PasswordHash = reader.GetString(paramPasswordHash);
                    User.NewPassword = reader.GetString(paramNewPassword);
                    User.Email = reader.GetString(paramEmail);
                    User.Id = reader.GetInt32(paramId);
                    User.Roles = new List<string>(reader.GetString(paramRoles).Split(",", StringSplitOptions.None));
                    users.Add(User);
                }

                connection.Close();
            }
            return users;
        }

        public int Insert(User User)
        {
            int userid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[Users] ([email],[passwordHash],[newPassword])output INSERTED.ID  VALUES  ('{User.Email}' ,'{User.PasswordHash}','{User.NewPassword}')";

                    command.Transaction = transaction;
                    userid = (int)command.ExecuteScalar();

                    //add userroles
                    AddRolesForUserId(userid, User.Roles, connection, transaction);
                    transaction.Commit();
                }
            }
            return userid;
        }

        public bool DeleteAllUsers()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.CommandText = "delete userroles";
                    command2.ExecuteScalar();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete users";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public void AddRolesForUserId(int userid, List<string> roles, SqlConnection connection, SqlTransaction transaction)
        {
            DeleteRolesByUserId(userid, connection, transaction);

            foreach (string role in roles)
            {
                var strrole = Enum.Parse(typeof(Roles), role,true);

                SqlCommand command2 = new SqlCommand();
                command2.Connection = connection;
                command2.CommandText = $"insert into [UserRoles](userid,roleid) values({userid},{(int)strrole})";

                command2.Transaction = transaction;

                var affected2 = command2.ExecuteNonQuery();
            }
        }

        public void DeleteRolesByUserId(int userid, SqlConnection connection, SqlTransaction transaction)
        {
            SqlCommand command2 = new SqlCommand();
            command2.Connection = connection;
            command2.Transaction = transaction;
            command2.CommandText = $"delete userroles where {paramUserId}={userid}";

            command2.ExecuteNonQuery();
        }

        public string Update(User User, int id)
        {
            User userFromDb = Get(id);

            if (userFromDb.Id == 0)
            {
                return "User with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"update users set email='{User.Email}', passwordHash='{User.PasswordHash}', newpassword='{User.NewPassword}' where id ={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    AddRolesForUserId(id, User.Roles, connection, transaction);

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public User Get(int userid)
        {
            User User = new User();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select userquery.uid,rolequery.roles,user from (select u.id as uid,STRING_AGG(name, ', ') as roles from roles r,userroles ur,users u where r.id=ur.RoleId and ur.Userid=u.id group by u.id) rolequery,(select u.id as uid,STRING_AGG(name, ', ') as roles from roles r,userroles ur,users u where r.id=ur.RoleId and ur.Userid=u.id group by u.id ) userquery where rolequery.uid=userquery.uid and userquery.uid={userid}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {                    
                    User.PasswordHash = reader.GetString(paramPasswordHash);
                    User.NewPassword = reader.GetString(paramNewPassword);
                    User.Email = reader.GetString(paramEmail);
                    User.Id = reader.GetInt32(paramId);
                    User.Roles = new List<string>(reader.GetString(paramRoles).Split(",", StringSplitOptions.None));
                    
                }
                connection.Close();
            }
            return User;
        }

        public User Get(string email)
        {
            User User = new User();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select userquery.*,rolequery.roles from (select u.email,STRING_AGG(name, ', ') as roles from roles r,userroles ur,users u where r.id=ur.RoleId and ur.Userid=u.id group by email) rolequery, (select u.email, u.id,u.passwordHash,u.newPassword from users u) userquery where rolequery.email=userquery.email and userquery.email='{email}'";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User.PasswordHash = reader.GetString(paramPasswordHash);
                    User.NewPassword = reader.GetString(paramNewPassword);
                    User.Email = reader.GetString(paramEmail);
                    User.Id = reader.GetInt32(paramId);
                    User.Roles = new List<string>(reader.GetString(paramRoles).Split(",", StringSplitOptions.None));

                }
                connection.Close();
            }
            
            if (User.Email == null)
                return null;

            return User;
        }  
    }

    public enum Roles
    {
        Admin = 1,
        Basic = 2
    }
}
