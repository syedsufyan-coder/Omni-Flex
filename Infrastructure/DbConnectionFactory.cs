using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace OmniFlex.Infrastructure
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetConnectionString("OracleDb")!;
        }

        public IDbConnection CreateConnection()
        {
            var conn = new OracleConnection(_connectionString);
    
            // This is the "Magic Line" for Dapper + Oracle
            conn.BindByName = true; 
    
            return conn;
        }
    }
}
