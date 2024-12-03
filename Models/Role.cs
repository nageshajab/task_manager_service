namespace TaskManager.Models
{
    public class Role
    {
        public int Id { get;set; }
        public string Name{ get; set; }
    }

    public class RoleSearch
    {
        public string Name { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public string SortBy { get; set; }
    }
}
