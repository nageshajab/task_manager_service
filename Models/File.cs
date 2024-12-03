namespace TaskManager.Models
{
    public class File
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string ParentFolder { get; set; }
        public string GoogleDrivePath { get; set; }
        public string AzurePath { get; set; }
    }

    public class FileSearch
    {
        public int UserId { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public string[] Tags { get; set; }
    }

    public class FileIndexViewModel
    {
        public List<File> ListOfFiles { get; set; }
        public List<string> Tags { get; set; }
        public FileSearch FileSearch { get; set; }

        public int TotalPages { get; set; }
    }
}
