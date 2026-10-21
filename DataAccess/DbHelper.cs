using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Student_Att.DataAccess
{
    // One place that knows how to create a database connection
    public static class DbHelper
    {
        public static SqlConnection GetConnection()
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["AttendanceDb"];

            if (setting == null)
            {
                throw new InvalidOperationException(
                    "Connection string 'AttendanceDb' is missing. Check ConnectionStrings.config.");
            }

            return new SqlConnection(setting.ConnectionString);
        }
    }
}
