using MongoDB.Driver;
using System.Configuration;

namespace MongoDbUtility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var pwd = Environment.GetEnvironmentVariable("mongodbpassword");
            var connstring = ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ToString();
            connstring = string.Format(connstring, pwd);
            var client = new MongoClient(connstring);

            MongoDbContext context= MongoDbContext.Create(client.GetDatabase("taskmanager"));
            
            List<TaskManager.Models.Task> tasks = context.Tasks.ToList();

            foreach (var task in tasks)
            {
                if (task.RepeatId==Guid.Empty)
                //repeatid
                task.RepeatId = new Guid();
                context.Tasks.Update(task);
            }
        }
    }
}
