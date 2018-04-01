using System;
using System.Data.SqlClient;
using Xunit;

namespace Sage.Tests
{
    static class Sql
    {
        public static readonly string ConnectionString;
        public static readonly string Skip;

        static Sql()
        {
            ConnectionString = GetConnectionStringOrDefault();

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    Skip = "";
                }
            }
            catch
            {
                ConnectionString = null;
                Skip = "Cannot connect to SQL Server";
            }
        }

        static string GetConnectionStringOrDefault()
        {
            string s = Environment.GetEnvironmentVariable("SAGE_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(s))
            {
                return "Data Source=.;Initial Catalog=master;Integrated Security=true;Connection Timeout=1;";
            }
            Console.WriteLine("Using Environment Connection String");
            return s;
        }
    }

    class SqlFactAttribute : FactAttribute
    {
        public SqlFactAttribute() => Skip = Sql.Skip;
    }
}
