using System;
using System.Threading.Tasks;

namespace MSGMicroservice.IDP.Infrastructure.Domains
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}