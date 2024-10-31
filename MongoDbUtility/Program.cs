using MongoDB.Driver;
using System.Configuration;
using Newtonsoft.Json;
using Models;

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

            var patientdata = File.ReadAllText("insurance_data.json");
            var patients = JsonConvert.DeserializeObject<List< Models.Patient>>(patientdata);

            var physiciandata = File.ReadAllText("physician_data.json");
            var physicians = JsonConvert.DeserializeObject<List<Models.Physicians>>(physiciandata);

            context.Patients.Add(patients.First());

            foreach(Physicians p in physicians)
            {
                context.Physicians.Add(p);
            }
            context.SaveChanges();
        }
    }
}
