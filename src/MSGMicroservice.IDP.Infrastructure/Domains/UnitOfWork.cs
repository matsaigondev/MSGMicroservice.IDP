using MSGMicroservice.IDP.Persistence;

namespace MSGMicroservice.IDP.Infrastructure.Domains;

public class UnitOfWork : IUnitOfWork
{
    private readonly MsgIdentityContext _context;

    public UnitOfWork(MsgIdentityContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Task<int> CommitAsync()
    {
        return _context.SaveChangesAsync();
    }
}