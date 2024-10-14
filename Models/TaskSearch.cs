namespace TaskManager.Models
{
    public class TaskSearch
    {
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DueFromDate { get; set; }
        public DateTime DueToDate { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set;} = 0;

    }
}
