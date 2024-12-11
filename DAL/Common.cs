using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Common
    {
        public static string ConnectionString = Environment.GetEnvironmentVariable("ApplicationDbContext");
    }
}
