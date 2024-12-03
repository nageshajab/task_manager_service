namespace TaskManager.Models
{
    public class BlogPost
    {       
        public int Id { get; set; }
        public int UserId { get; set; }  
        public string Title { get; set; }
        public string Description { get; set; }
        public string RepositoryUrl { get; set; }
        public string BlogPostUrl{ get; set; }
        public List< string> Tags { get; set; }
    }

    public class BlogPost1
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RepositoryUrl { get; set; }
        public string BlogPostUrl { get; set; }
        public string[] Tags { get; set; }
    }

    public class BlogPostSearch
    {
        public int UserId { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class BlogPostIndexViewModel
    {
        public List<BlogPost> ListOfBlogPosts{ get; set; }
        public BlogPostSearch BlogPostSearch { get; set; }
        public int TotalPages { get; set; }
    }
}
