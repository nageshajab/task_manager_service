using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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
        public DbSet<SugarReading> SugarReadings { get; init; }
        public DbSet<File>? Files{ get; init; }
        public DbSet<Url> Urls{ get; init; }

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
