using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DatabaseContext
    {
        protected readonly string connectionString;

        public DatabaseContext()
        {
            // Always use "MyDb"
            connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
        }


        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
