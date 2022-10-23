using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Persistence;

namespace MSGMicroservice.IDP.Infrastructure.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MsgIdentityContext _dbContext;
    private readonly Lazy<IPermissionRepository> _permissionRepository;//use Lazy
    private readonly IMapper _mapper;

    public RepositoryManager(IUnitOfWork unitOfWork, MsgIdentityContext dbContext, 
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        UserManager = userManager;
        RoleManager = roleManager;
        _mapper = mapper;

        _permissionRepository = new Lazy<IPermissionRepository>(() =>
            new PermissionRepository(_dbContext, _unitOfWork, UserManager, _mapper));//chinh thuc su dung moi chay
    }

    public UserManager<User> UserManager { get; }
    public RoleManager<IdentityRole> RoleManager { get; }
    
    public IPermissionRepository Permission => _permissionRepository.Value;

    public Task<int> SaveAsync()
        => _unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync()
        => _dbContext.Database.BeginTransactionAsync();

    public Task EndTransactionAsync()
        => _dbContext.Database.CommitTransactionAsync();

    public void RollbackTransaction()
        => _dbContext.Database.RollbackTransactionAsync();
}