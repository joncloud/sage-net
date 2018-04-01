using System;
using System.Data.SqlClient;
using Xunit;

namespace Sage.Tests
{
    class SqlAttribute : FactAttribute
    {
        public static readonly string ConnectionString;
        static readonly string _skip;

        public SqlAttribute() => Skip = _skip;

        static SqlAttribute()
        {
            ConnectionString = GetConnectionStringOrDefault();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    _skip = "";
                }
            }
            catch
            {
                _skip = "Cannot connect to SQL Server";
            }
        }

        static string GetConnectionStringOrDefault()
        {
            string s = Environment.GetEnvironmentVariable("SAGE_CONNECTION_STRING");
            return string.IsNullOrWhiteSpace(s) 
                ? "Data Source=.;Initial Catalog=master;Integrated Security=true;Connection Timeout=1;"
                : s;
        }
    }
}
