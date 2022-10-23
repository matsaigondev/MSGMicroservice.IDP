using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MSGMicroservice.IDP.Infrastructure.Domains
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            //_connectionString = _configuration.GetConnectionString(RelationDBFactory.GetDB(401).GetConnection());
            _connectionString = _configuration.GetConnectionString("ConnectionString");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}