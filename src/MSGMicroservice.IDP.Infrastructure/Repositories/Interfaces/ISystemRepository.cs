using MSGMicroservice.IDP.Infrastructure.Common;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces
{
    public interface ISystemRepository
    {
        Task<PagedResult<LogDTO>> GetLogsByPaging(GetCommonPaging request);
    }
}