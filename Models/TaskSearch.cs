namespace TaskManager.Models
{
    public class TaskSearch
    {
        public int UserId { get; set; } 
        public string Status { get; set; } = string.Empty;
        public DateTime DueFromDate { get; set; }
        public DateTime DueToDate { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set;} = 0;
        public List<Task> Tasks { get; set; }
     
    }
}
