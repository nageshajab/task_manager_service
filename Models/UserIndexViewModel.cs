using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class UserIndexViewModel
    {
        public List<TaskManager.Models.User> ListOfUsers { get; set; }
        public UserSearch UserSearch { get; set; }
        public int TotalPages { get; set; }
    }

    public class UserSearch
    {
        public string Name { get; set; }    
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public string SortBy { get; set; }
    }
}
