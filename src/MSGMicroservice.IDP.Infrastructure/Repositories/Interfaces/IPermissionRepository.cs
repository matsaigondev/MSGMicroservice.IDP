using System.Collections.Generic;
using System.Threading.Tasks;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Infrastructure.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, long>
{
    Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId);
    Task<PermissionViewModel?> CreatePermission(string roleId, PermissioAddModel model);
    Task DeletePermission(string roleId, string function, string command);
    Task UpdatePermissionsByRoleId(string roleId, IEnumerable<PermissioAddModel> permissionCollection);
    Task<IEnumerable<PermissionUserViewModel>> GetPermissionsByUser(User user);
    Task<IReadOnlyList<PermissionDTO>> GetPermissions();
    Task<IReadOnlyList<PermissionDTO>> GetPermissionsByRoleId(string roleId);
    Task<List<string>> GetPermissionsByRoleId0(string roleId);
    Task<IReadOnlyList<PermissionRoleVm>> GetPermissionRolesByRole(string roleId);
}