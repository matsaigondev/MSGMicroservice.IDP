using Dapper;
using Microsoft.Extensions.Configuration;
using MSGMicroservice.IDP.Infrastructure.Common;
using MSGMicroservice.IDP.Infrastructure.Persistence;
using MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces;
using MSGMicroservice.IDP.Infrastructure.ViewModels;
using System.Data;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public class SystemRepository : DapperADO, ISystemRepository
    {
        private readonly IRepositoryBaseS _repository;
        public SystemRepository(IConfiguration configuration, IRepositoryBaseS repository) : base(configuration)
        {
            _repository = repository;
        }

        public async Task<PagedResult<LogDTO>> GetLogsByPaging(GetCommonPaging request)
        {
            CancellationToken cancellationToken = new CancellationToken();
            var sql = "proc_getlogs_bypaging";
            var parameters = new DynamicParameters();
            parameters.Add("@pageNumber", request.PageIndex, DbType.Int32);
            parameters.Add("@pageSize", request.PageSize, DbType.Int32);
            parameters.Add("@filter", string.IsNullOrEmpty(request.Keyword) ? "" : request.Keyword, DbType.String);
            parameters.Add("@filter1", string.IsNullOrEmpty(request.Filter1) ? "" : request.Filter1, DbType.String);
            parameters.Add("@filter2", string.IsNullOrEmpty(request.Filter2) ? "" : request.Filter2, DbType.String);
            parameters.Add("@filter3", string.IsNullOrEmpty(request.Filter3) ? "" : request.Filter3, DbType.String);
            parameters.Add("@fromDate", string.IsNullOrEmpty(request.FromDate) ? "" : request.FromDate, DbType.String);
            parameters.Add("@toDate", string.IsNullOrEmpty(request.ToDate) ? "" : request.ToDate, DbType.String);
            parameters.Add("@hospitalId",
                int.Parse(string.IsNullOrEmpty(request.ParentId) ? "0" : request.ParentId), DbType.Int32);
            parameters.Add("@totalRow", 0, dbType: DbType.Int32,
                direction: ParameterDirection.Output);

            var getlist = await _repository.QueryStoreProcedureAsync<LogDTO>(cancellationToken, sql, ConnectionStringLog, parameters);
            var output = parameters.Get<int>("@totalRow");
            var pagedResult = new PagedResult<LogDTO>()
            {
                TotalRecords = output,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Data = getlist.ToList()
            };
            return pagedResult;
        }
    }
}