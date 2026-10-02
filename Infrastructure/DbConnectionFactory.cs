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

            // Debug — is string being properly returned from appsettings.json or not
            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("CONNECTION STRING IS NULL OR EMPTY — Check appsettings.json");
        }

		public IDbConnection CreateConnection()
		{
			// Create Oracle connection from appsettings.json connection string
			var conn = new OracleConnection(_connectionString);
			conn.BindByName = true;

			// Open connection, set default schema, then return open connection
			// Dapper will close it automatically via 'using' statement in repositories
			conn.Open();
			using var cmd = conn.CreateCommand();
			cmd.CommandText = "ALTER SESSION SET CURRENT_SCHEMA = OMNI_FLEX";
			cmd.ExecuteNonQuery();

			// Do NOT close here — return open connection for Dapper to use
			return conn;
		}
	}
}