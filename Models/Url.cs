namespace TaskManager.Models
{
    public class Url
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Link{ get; set; }       
        public string Description { get; set; }
        public List< string> Tags { get; set; }
    }

    public class UrlSearch
    {
        public int UserId { get; set; } 
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public string Tags { get; set; }
    }

    public class UrlIndexViewModel
    {
        public List<Url> ListOfUrls { get; set; }
        public List<string> Tags { get; set; }
        public UrlSearch UrlSearch { get; set; }

        public int TotalPages { get; set; }
    }
}
