using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListService.Models
{
    public class User
    {
        public string email { get; set; }

        public string ObjectId { get; set; }

        public string Surname { get; set; }

        public string DisplayName { get; set; }

        public string GivenName { get; set; }

        public string ClientCode { get; set; }  

        public string TaxId { get; set; }
    }
}
