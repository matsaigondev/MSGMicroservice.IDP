namespace MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces
{
    public interface IRepositoryBaseS
    {
        Task<int> ExecuteAsync(CancellationToken cancellationToken, string sqlQuery, string connectionString, object parrametter = null);

        Task<int> ExecuteSPAsync(CancellationToken cancellationToken, string procedureName, string connectionString, object parrametter = null);

        Task<List<T>> QueryStoreProcedureAsync<T>(CancellationToken cancellationToken, string procedureName, string connectionString, object parrametter = null);

        Task<List<T>> QueryAsync<T>(CancellationToken cancellationToken, string sqlQuery, string connectionString, object parametter = null);
    }
}