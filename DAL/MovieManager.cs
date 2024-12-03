using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class MovieManager
    {
        public string ConnectionString = Environment.GetEnvironmentVariable("ApplicationDbContext");

        public List<Movie> ListMoviesByUserId(MovieSearch movieSearch)
        {
            List<Movie> movies = [];

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();


                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = $"select * from movies where userid={movieSearch.UserId}";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Movie movie = new()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Genre = (Genre)Enum.Parse(typeof(Genre), reader.GetString("genre")),
                        Rating = reader.GetInt32("rating"),
                        Description = reader.GetString("description"),

                        Language = (Langauge)Enum.Parse(typeof(Langauge), reader.GetString("language")),
                        Actors = reader.GetString("actors").Split(",", StringSplitOptions.None).ToList(),
                        UserId = reader.GetInt32("userid"),
                        Tags = reader.GetString("tags").Split(",", StringSplitOptions.None).ToList()
                    };

                    movies.Add(movie);
                }

                connection.Close();
            }
            return movies;
        }

        public int Insert(Movie movie)
        {
            int movieid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[movies] ([name],[genre],[rating],[description],[language],[actors],[userid],[tags]) VALUES ('{movie.Name}','{movie.Genre}',{movie.Rating},'{movie.Description}','{movie.Language}','{movie.Actors}',{movie.UserId},''))";

                    command.Transaction = transaction;
                    movieid = (int)command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return movieid;
        }

        public bool DeleteAllMovies()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete movies";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string Update(Movie movie, int id)
        {
            Movie movieFromDb = Get(id);

            if (movieFromDb.Id == 0)
            {
                return "movie with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[movies] SET [name] = '{movie.Name}',[genre] = '{movie.Genre}',[rating] = {movie.Rating},[description] = '{movie.Description}',[language] = '{movie.Language}',[actors] = '{movie.Actors}',[userid] = {movie.UserId},[tags] = '{movie.Tags}' WHERE id={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public Movie Get(int id)
        {
            Movie movie = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from movies where id={id}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                     movie = new()
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Genre = (Genre)Enum.Parse(typeof(Genre), reader.GetString("genre")),
                        Rating = reader.GetInt32("rating"),
                        Description = reader.GetString("description"),

                        Language = (Langauge)Enum.Parse(typeof(Langauge), reader.GetString("language")),
                        Actors = reader.GetString("actors").Split(",", StringSplitOptions.None).ToList(),
                        UserId = reader.GetInt32("userid"),
                        Tags = reader.GetString("tags").Split(",", StringSplitOptions.None).ToList()
                    };
                }
                connection.Close();
            }
            return movie;
        }

        public bool DeleteMovie(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                   SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete movies where id=" + id;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }
    }
}
