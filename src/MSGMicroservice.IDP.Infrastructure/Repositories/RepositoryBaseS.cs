using Dapper;
using Microsoft.Data.SqlClient;
using MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces;
using System.Data;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public class RepositoryBaseS : IRepositoryBaseS
    {
        public async Task<int> ExecuteAsync(CancellationToken cancellationToken, string sqlQuery, string connectionString, object parrametter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        await connection.OpenAsync();
                    return await connection.ExecuteAsync(sqlQuery, parrametter);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<int> ExecuteSPAsync(CancellationToken cancellationToken, string procedureName, string connectionString, object parrametter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        await connection.OpenAsync();
                    return await connection.ExecuteAsync(procedureName, parrametter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<List<T>> QueryAsync<T>(CancellationToken cancellationToken, string sqlQuery, string connectionString, object parametter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        await connection.OpenAsync(cancellationToken);
                    return (await connection.QueryAsync<T>(sqlQuery, parametter)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<List<T>> QueryStoreProcedureAsync<T>(CancellationToken cancellationToken, string procedureName, string connectionString, object parrametter = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                        await connection.OpenAsync();
                    return (await connection.QueryAsync<T>(procedureName, parrametter, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}