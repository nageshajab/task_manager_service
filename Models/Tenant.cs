using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rent { get; set; }
        public string RoomLocation { get; set; }
    }

    public class Rent:Tenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class RentSearch
    {
        public int UserId { get; set; }

        //public string Status { get; set; } = string.Empty;
        //public string DueFromDate { get; set; }
        //public string DueToDate { get; set; }

        public string SortBy { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public List<Rent> Rents { get; set; }
        public List<Tenant> Tenants{ get; set; }
    }

    public class RentIndexViewModel
    {
        public List<TaskManager.Models.Rent> Rents{ get; set; }
        public RentSearch RentSearch { get; set; }
        public int TotalPages { get; set; }
    }
}
