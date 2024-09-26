using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class TaskIndexViewModel
    {
        public List<TaskManager.Models.Task> ListOfTasks { get; set; }
        public TaskSearch TaskSearch { get; set; }
        public int TotalPages { get; set; }
    }
}
