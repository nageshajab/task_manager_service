using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Security.Authentication;

namespace DAL
{
    public class SugarReadingManager
    {
        public List<SugarReading> List(int userid)
        {
            List<SugarReading> SugarReadings = new List<SugarReading>();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from SugarReadings where userid={userid}";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SugarReading SugarReading = new SugarReading();
                    SugarReading.Id = reader.GetInt32("id");
                    SugarReading.UserId = reader.GetInt32("userid");
                    SugarReading.Fasting = reader.GetInt32("fasting");
                    SugarReading.PP = reader.GetInt32("pp");
                    SugarReading.Date=reader.GetDateTime("date");
                    SugarReading.Weight = reader.GetInt32("weight");
                    SugarReading.Medicines=reader.GetString("medicines").Split(',',StringSplitOptions.None).ToList();

                    SugarReadings.Add(SugarReading);
                }

                connection.Close();
            }
            return SugarReadings;
        }

        public int Insert(SugarReading SugarReading)
        {
            int id;
            int SugarReadingid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[SugarReadings] ([userid],[fasting],[pp],[date],[weight],[medicines]) VALUES ({SugarReading.UserId},{SugarReading.Fasting},{SugarReading.PP},{SugarReading.Date},{SugarReading.Weight},'{SugarReading.Medicines}')";

                    command.Transaction = transaction;
                    SugarReadingid = (int)command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return SugarReadingid;
        }

        public bool DeleteAllSugarReadings()
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
                    command2.CommandText = "delete SugarReadings";
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public bool DeleteSugarReadingByid(int id)
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
                    command2.CommandText = "delete SugarReadings where id = " + id;
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string Update(SugarReading SugarReading, int id)
        {
            SugarReading userFromDb = Get(id);

            if (userFromDb.Id == 0)
            {
                return "SugarReading with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[SugarReadings] SET [userid] = {SugarReading.UserId},[fasting] = {SugarReading.Fasting},[pp] = {SugarReading.PP},[date] ='{SugarReading.Date}' ,[weight] ={SugarReading.Weight} ,[medicines] ='{SugarReading.Medicines}' WHERE id={SugarReading.Id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public SugarReading Get(int id)
        {
            SugarReading SugarReading = new SugarReading();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from SugarReadings where id='{id}'";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SugarReading.Id = reader.GetInt32("id");
                    SugarReading.UserId = reader.GetInt32("userid");
                    SugarReading.Fasting = reader.GetInt32("fasting");
                    SugarReading.PP = reader.GetInt32("pp");
                    SugarReading.Date = reader.GetDateTime("date");
                    SugarReading.Weight = reader.GetInt32("weight");
                    SugarReading.Medicines = reader.GetString("medicines").Split(',', StringSplitOptions.None).ToList();

                }
                connection.Close();
            }   
            return SugarReading;
        }
    }
}
