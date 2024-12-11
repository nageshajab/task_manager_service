using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class UrlManager
    {
        public List<Url> List(int Urlid)
        {
            List<Url> Urls = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from url where Urlid= {Urlid}";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Url Url = new Url();
                    Url.Id = reader.GetInt32("id");
                    Url.UserId = reader.GetInt32("userid");
                    Url.Link = reader.GetString("link");
                    Url.Description = reader.GetString("description");
                    Url.Tags = reader.GetString("tags").Split(",", StringSplitOptions.None).ToList();

                    Urls.Add(Url);
                }

                connection.Close();
            }
            return Urls;
        }

        public void Insert(Url Url)
        {
            int Urlid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[url] ([userid],[link],[description],[tags]) VALUES ({Url.UserId},'{Url.Link}','{Url.Description}','{string.Join(",", Url.Tags)}')";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public bool DeleteAllUrls()
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
                    command2.CommandText = "delete url";
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public bool DeleteUrl(int id)
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
                    command2.CommandText = $"delete url where id ={id}";
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string Update(Url Url, int id)
        {
            Url UrlFromDb = Get(id);

            if (UrlFromDb.Id == 0)
            {
                return "Url with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[url] SET [userid] = {Url.UserId} ,[link] = '{Url.Link}',[description] ='{Url.Description}',[tags] = '{Url.Tags}' WHERE id={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public Url Get(int Urlid)
        {
            Url Url = new Url();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from url where id={Urlid}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Url.Id = reader.GetInt32("id");
                    Url.UserId = reader.GetInt32("userid");
                    Url.Link = reader.GetString("link");
                    Url.Description = reader.GetString("description");
                    Url.Tags = reader.GetString("tags").Split(",", StringSplitOptions.None).ToList();
                }
                connection.Close();
            }
            return Url;
        }
    }

}
