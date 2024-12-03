using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace DAL
{
    public class RoleManager
    {
        public string ConnectionString = Environment.GetEnvironmentVariable("ApplicationDbContext");

        public List<Role> List()
        {
            List<Role> roles = new List<Role>();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "select * from roles";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Role Role = new Role();
                    Role.Id = reader.GetInt32("id");
                    Role.Name = reader.GetString("name");
                    roles.Add(Role);
                }

                connection.Close();
            }
            return roles;
        }

        public int Insert(Role Role)
        {
            int id;
            int roleid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[roles] ([name]) output INSERTED.ID  VALUES  ('{Role.Name}')";

                    command.Transaction = transaction;
                    roleid = (int)command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return roleid;
        }

        public bool DeleteAllRoles()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.CommandText = "delete roles";
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public bool DeleteRoleByid(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.CommandText = "delete roles where id = " + id;
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string Update(Role Role, int id)
        {
            Role userFromDb = Get(id);

            if (userFromDb.Id == 0)
            {
                return "Role with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"update roles set name='{Role.Name}' where id ={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public Role Get(int id)
        {
            Role Role = new Role();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from roles where id='{id}'";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Role.Name= reader.GetString("name");
                    Role.Id = id;
                }
                connection.Close();
            }

            
            return Role;
        }


    }


}
