using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ItemWebapi.DataAccess
{
    public class DataAccessLayer
    {
        public static SqlConnection CreateConnection()
        {
            string connectionString = @"Server=SAMHAN\SQLEXPRESS;Database=techfix;Trusted_Connection=True;";
            return new SqlConnection(connectionString);
        }
    }
}