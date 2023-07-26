using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace test.Models
{
    public class DBConnectionStr
    {
        internal static string ConnStr;

        public void GetConnStr(IConfiguration configuration) 
        {
            ConnStr = configuration.GetConnectionString("SqlConnection");
        }
    }
}