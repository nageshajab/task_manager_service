namespace TaskManager.Models
{
    public class Movie
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public Genre Genre { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; } = string.Empty;
        public Langauge Language { get; set; } 
        public List<string> Actors { get; set; }
        public List<string> Tags { get; set; }
        public int UserId { get; set; }
    }
 
    public enum Langauge
    {
        Hindi=0,
        English = 1,
        Tamil = 2,
        Telugu = 3,
        Malayalam = 4,
        Kannada = 5,
        Marathi = 6,

    }

    public enum Genre
    {
        Suspense = 0,
        Action = 1,
        Comedy = 2,
        Romance = 3,
        Drama = 4,
        Horror = 5,
        SciFi = 6,
        Documentary = 7,
        Animation = 8,
    }

    public class MovieSearch
    {
        public int UserId { get; set; } 
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class MovieIndexViewModel
    {
        public List<TaskManager.Models.Movie>? ListOfMovies { get; set; }
        public MovieSearch? MovieSearch { get; set; }
        public int TotalPages { get; set; }
    }
}
