using System.Data;
using Microsoft.Data.SqlClient;

namespace PasswordlessApi.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        public readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidCastException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}