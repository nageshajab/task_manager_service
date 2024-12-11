using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Task = TaskManager.Models.Task;

namespace DAL
{
    public class RentManager
    {
        string listtaskquery = "select * from (select * from Task where userid =@userid and status!='Completed'  and (@fromquery and @toquery) union select * from Task where userid =@userid and duedate < '@duedate' and status!='Completed') a ORDER BY duedate, id OFFSET @skiprecords ROWS FETCH NEXT 10 ROWS ONLY";

        string countquery = "select count(*) from (select * from Task where userid =@userid and status!='Completed'  and (@fromquery and @toquery) union select * from Task where userid =@userid and duedate < '@duedate' and status!='Completed') a";

        public TaskSearch ListTasksByUserId(TaskSearch TaskSearch)
        {
            string from = "1=1", to = "1=1";
            DateTime duefrom = TaskSearch.DueFromDate == null ? DateTime.MinValue : DateTime.Parse(TaskSearch.DueFromDate);
            DateTime dueto = TaskSearch.DueToDate == null ? DateTime.MinValue : DateTime.Parse(TaskSearch.DueToDate);

            string duefromdate = $"{duefrom.Year}-{duefrom.Month}-{duefrom.Date.Day} 00:00:00.000";
            string duetodate = $"{dueto.Year}-{dueto.Month}-{dueto.Date.Day} 00:00:00.000";

            if (duefrom != DateTime.MinValue)
                from = $" duedate > '{duefromdate}'";

            if (dueto != DateTime.MinValue)
                to = $" duedate < '{duetodate}'";

            List<Task> Tasks = [];
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand totalRecords = new()
                {
                    Connection = connection,
                    CommandText = $"{countquery.Replace("@userid", TaskSearch.UserId.ToString())
                    .Replace("@fromquery", from)
                    .Replace("@toquery", to)
                    .Replace("@duedate", duefromdate)}"
                };
                TaskSearch.TotalRecords = (int)totalRecords.ExecuteScalar();

                int startrecord = (TaskSearch.PageNumber - 1) * 10;
                SqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = $"{listtaskquery.Replace("@userid", TaskSearch.UserId.ToString())
                    .Replace("@fromquery", from)
                    .Replace("@toquery", to)
                    .Replace("@duedate", duefromdate)
                    .Replace("@skiprecords", startrecord.ToString())}"
                };

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Task Task = new();
                    Task.Id = reader.GetInt32("id");
                    Task.Title = reader.GetString("title");
                    Task.Description = reader.GetString("description");
                    Task.DueDate = reader.GetDateTime("duedate");
                    Task.Priority = (Priority)Enum.Parse(typeof(Priority), reader.GetString("priority"));
                    Task.Status = (Status)Enum.Parse(typeof(Status), reader.GetString("status"));
                    Task.UserId = reader.GetInt32("userid");

                    Task.EndDate = reader.GetDateTime("enddate");
                    Task.RepeatType = (RepeatType)Enum.Parse(typeof(RepeatType), reader.GetString("repeatType"));
                    Task.Type = reader.GetString("type");
                    Task.SubType = reader.GetString("subtype");

                    Tasks.Add(Task);
                }

                connection.Close();
            }

            TaskSearch.Tasks = Tasks;

            return TaskSearch;
        }

        public void Insert(Task Task)
        {
            using SqlConnection connection = new();
            connection.ConnectionString =Common. ConnectionString;
            connection.Open();
            using var transaction = connection.BeginTransaction();
            SqlCommand command = new()
            {
                Connection = connection,
                CommandText = $"INSERT INTO [dbo].[Task] ([title],[description],[duedate],[priority],[status],[userid],[enddate],[repeatType],[type],[subtype]) VALUES ('{Task.Title}','{Task.Description}','{Task.DueDate}','{Task.Priority}','{Task.Status}',{Task.UserId},'{Task.EndDate}','{Task.RepeatType}','{Task.Type}','{Task.SubType}')",

                Transaction = transaction
            };
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public bool DeleteAllTasks()
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
                    command.CommandText = "delete Task";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string Update(Task Task, int id)
        {
            Task TaskFromDb = Get(id);

            if (TaskFromDb.Id == 0)
            {
                return "Task with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[Task] SET [title] = '{Task.Title}' ,[description] = '{Task.Description}' ,[duedate] ='{Task.DueDate}',[priority] = '{Task.Priority}' ,[status] = '{Task.Status}',[userid] = {Task.UserId}, [enddate] = '{Task.EndDate}', [repeatType] ='{Task.RepeatType}',[type]='{Task.Type}',[subtype]='{Task.SubType}' WHERE id={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public string updatetaskstatus(int id, string status)
        {
            Task TaskFromDb = Get(id);

            if (TaskFromDb.Id == 0)
            {
                return "Task with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Common.ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[Task] SET [status] = '{status}' WHERE id={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public Task Get(int id)
        {
            Task Task = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =Common. ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from Task where id={id}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Task = new()
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        DueDate = reader.GetDateTime("duedate"),
                        Priority = (Priority)Enum.Parse(typeof(Priority), reader.GetString("priority")),
                        Status = (Status)Enum.Parse(typeof(Status), reader.GetString("status")),
                        UserId = reader.GetInt32("userid"),
                        EndDate = reader.GetDateTime("enddate"),
                        RepeatType = (RepeatType)Enum.Parse(typeof(RepeatType), reader.GetString("repeatType")),
                        Type = reader.GetString("type"),
                        SubType = reader.GetString("subtype")
                    };
                }
                connection.Close();
            }
            return Task;
        }

        public bool DeleteTask(int id)
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
                    command.CommandText = "delete Task where id=" + id;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }
    }
}
