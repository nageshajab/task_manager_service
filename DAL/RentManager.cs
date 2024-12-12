using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Models;
using TaskManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DAL
{
    public class RentManager
    {
        string insertTenant = "INSERT INTO[dbo].[Tenant] ([name],[rent],[roomlocation])             VALUES(@name,@rent,@roomlocation)";
        
        string insertRent = "INSERT INTO[dbo].[Rents]([tenantid],[amount] ,[date])        VALUES(@tenantid,@amount,@date)";

        public RentSearch ListRentsByUserId( RentSearch RentSearch)
        {
            string from = "1=1", to = "1=1";
          
            List<Rent> Rents = [];
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                string countquery = "select count(r.id) from rents r, tenant t where r.tenantid=t.id";
                string listquery = "select * from rents r, tenant t where r.tenantid=t.id";
                SqlCommand totalRecords = new()
                {
                    Connection = connection,
                    CommandText = $"{countquery.Replace("@userid", RentSearch.UserId.ToString())}"
                };
                RentSearch.TotalRecords = (int)totalRecords.ExecuteScalar();

                int startrecord = (RentSearch.PageNumber - 1) * 10;
                SqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = $"{listquery.Replace("@userid", RentSearch.UserId.ToString())
                    .Replace("@skiprecords", startrecord.ToString())}"
                };

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                   Rent Rent = new();
                    Rent.Id = reader.GetInt32("id");
                    Rent.TenantId = reader.GetInt32("TenantId");
                    Rent.Amount = reader.GetInt32("Amount");
                    Rent.Date = reader.GetDateTime("date");
                    Rent.Name= reader.GetString("name");
                    Rent.Rent =  reader.GetInt32("rent");
                    Rent.RoomLocation= reader.GetString("RoomLocation");

                    Rents.Add(Rent);
                }

                connection.Close();
            }

            RentSearch.Rents = Rents;

            return RentSearch;
        }

        public RentSearch ListTenantsByUserId(RentSearch RentSearch)
        {
            string from = "1=1", to = "1=1";

            List<Tenant> tenants= [];
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Common.ConnectionString;
                connection.Open();

                string countquery = $"select count(*) from tenant where userid={RentSearch.UserId}";

                string listquery = "select * from tenant where userid=@userid";

                SqlCommand totalRecords = new()
                {
                    Connection = connection,
                    CommandText = $"{countquery}"
                };
                RentSearch.TotalRecords = (int)totalRecords.ExecuteScalar();

                int startrecord = (RentSearch.PageNumber - 1) * 10;
                SqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = $"{listquery.Replace("@userid", RentSearch.UserId.ToString()
                    )}"
                };

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Tenant tenant = new();
                    tenant.Id = reader.GetInt32("id");
                    tenant.Name = reader.GetString("name");
                    tenant.Rent= reader.GetInt32("rent");
                    tenant.RoomLocation = reader.GetString("RoomLocation");

                    tenants.Add(tenant);
                }

                connection.Close();
            }

            RentSearch.Tenants = tenants;

            return RentSearch;
        }

        public void InsertTenant(Tenant tenant)
        {
            using SqlConnection connection = new();
            connection.ConnectionString =Common. ConnectionString;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            SqlCommand command = new()
            {
                Connection = connection,
                CommandText = $"{insertTenant.Replace("@name",tenant.Name)
                .Replace("@rent",tenant.Rent.ToString())
                .Replace("@roomlocation",tenant.RoomLocation)}",

                Transaction = transaction
            };
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void InsertRent(Rent rent)
        {
            using SqlConnection connection = new();
            connection.ConnectionString = Common.ConnectionString;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            SqlCommand command = new()
            {
                Connection = connection,
                CommandText = $"{insertRent.Replace("@tenantid", rent.TenantId.ToString())
                .Replace("@amount", rent.Amount.ToString())
                .Replace("@date", rent.Date.ToString())}",

                Transaction = transaction
            };
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public bool DeleteAllRents()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete Rent";
                    command.ExecuteScalar();

                    SqlCommand command2 = new SqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.CommandText = "delete tenant";
                    command2.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string UpdateRent(Rent Rent, int id)
        {
            string updaterentquery = "update rent set tenantid=@tenantid, amount=@amount, date='@date' where id=@id";
            Rent RentFromDb = Get(id);

            if (RentFromDb.Id == 0)
            {
                return "Rent with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"{updaterentquery.Replace("@tenantid",Rent.TenantId.ToString())
                        .Replace("@amount",Rent.Amount.ToString())
                        .Replace("@date",Rent.Date.ToString())
                        .Replace("@id",Rent.Id.ToString())}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public string UpdateTenant(Tenant tenant, int id)
        {
            string updatetenantquery = "update tenant set name='@name', rent=@rent, roomlocation='@roomlocation' where id=@id";
            Tenant tenantFromDb = Get(id);

            if (tenantFromDb.Id == 0)
            {
                return "Tenant with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Common.ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"{updatetenantquery.Replace("@name", tenant.Name)
                        .Replace("@rent", tenant.Rent.ToString())
                        .Replace("@roomlocation", tenant.RoomLocation)
                        .Replace("@id", tenant.Id.ToString())}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public Rent Get(int id)
        {
            Rent Rent = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from Rent where id={id}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Rent = new()
                    {
                        Id = reader.GetInt32("id"),
                        TenantId= reader.GetInt32("tenantid"),
                        Amount= reader.GetInt32("amount"),
                        Date= reader.GetDateTime("date")
                    };
                }
                connection.Close();
            }
            return Rent;
        }

        public Tenant GetTenant(int id)
        {
            Tenant tenant = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Common.ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from tenant where id={id}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tenant = new()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Rent= reader.GetInt32("rent"),
                        RoomLocation= reader.GetString("roomlocation")
                    };
                }
                connection.Close();
            }
            return tenant;
        }

        public bool DeleteRent(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete Rent where id=" + id;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public bool DeleteTenant(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Common.ConnectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete tenant where id=" + id;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }
    }
}
