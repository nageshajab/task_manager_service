namespace TaskManager.Models
{
    public class SugarReading
    {   
        public int Id { get; set; }        
        public int UserId { get; set; }
        public int Fasting { get; set; }
        public int PP { get; set; }
        public DateTime Date { get; set; }
        public int Weight { get; set; }
        public List<String> Medicines { get; set; }
    }

    public class SugarReadingSearch
    {
        public int UserId { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
    }

    public class SugarReadingIndexViewModel
    {
        public List<SugarReading> ListOfSugarReadings { get; set; }
        public SugarReadingSearch SugarReadingSearch{ get; set; }
        public int TotalPages { get; set; }
    }
}
