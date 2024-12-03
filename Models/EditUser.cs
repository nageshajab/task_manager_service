﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 // Added missing using directive

namespace TaskManager.Models
{
    public class EditUser
    {
        public string Id { get; set; } // Added missing property
        public string UserName { get; set; }
        public List<string> AllRoles { get; set; } = new List<string>(); // Initialized to avoid CS8618
        public IList<string> SelectedRoles { get; set; } = new List<string>(); // Initialized to avoid CS8618

        public string NewRoles { get; set; } = string.Empty; // Initialized to avoid CS8618
    }
}
