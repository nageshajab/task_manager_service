using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rent { get; set; }
        public string RoomLocation { get; set; }
    }

    public class Rent
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }

}
