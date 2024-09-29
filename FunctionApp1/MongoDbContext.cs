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
    public class MongoDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; init; }
        public DbSet<Role> Roles { get; init; }
        public DbSet<User> Users { get; init; }
        public DbSet<TaskManager.Models.Task > Tasks { get; init; }
        public DbSet<BlogPost> BlogPosts{ get; init; }
        public DbSet<Rent > Rents { get; init; }
        public DbSet<SugarReading> SugarReadings { get; init; }

        public static MongoDbContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<MongoDbContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);

        public MongoDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
