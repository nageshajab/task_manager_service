using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using TaskManager.Models;

namespace FunctionApp1
{

    //    var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
    //if (connectionString == null)
    //{
    //    Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
    //    Environment.Exit(0);
    //}
    //var client = new MongoClient(connectionString);
    //var db = MflixDbContext.Create(client.GetDatabase("sample_mflix"));
    //db.Database.EnsureCreated();
    //var movie = db.Movies.First(m => m.Title == "Back to the Future");
    //Console.WriteLine(movie.Plot);
    public class MongoDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; init; }
        public DbSet<Role> Roles { get; init; }
        public DbSet<User> Users { get; init; }
        public DbSet<TaskManager.Models.Task > Tasks { get; init; }
        public DbSet<BlogPost> BlogPosts{ get; init; }

        public static MongoDbContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<MongoDbContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public MongoDbContext(DbContextOptions options)
            : base(options)
        {
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //   // modelBuilder.Entity<Movie>().ToCollection("taskmanager");
        //}
    }

}
