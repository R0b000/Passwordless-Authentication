using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace API.Shared.Configuration
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? _configuration["DefaultConnection"]
                ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidCastException("Connection string 'DefaultConnection' not found in configuration or environment variables.");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
