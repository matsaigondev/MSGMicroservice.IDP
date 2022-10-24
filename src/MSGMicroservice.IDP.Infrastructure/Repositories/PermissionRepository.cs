using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Infrastructure.ViewModels;
using MSGMicroservice.IDP.Persistence;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public class PermissionRepository : RepositoryBase<Permission, long>, IPermissionRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public PermissionRepository(MsgIdentityContext dbContext, IUnitOfWork unitOfWork, UserManager<User> userManager,
            IMapper mapper) : base(dbContext, unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId);
            var result = await QueryAsync<PermissionViewModel>("Get_Permission_ByRoleId", parameters);
            return result;
        }

        public async Task<PermissionViewModel?> CreatePermission(string roleId, PermissioAddModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            parameters.Add("@function", model.Function, DbType.String);
            parameters.Add("@command", model.Command, DbType.String);
            parameters.Add("@newID", dbType: DbType.Int64, direction: ParameterDirection.Output);

            var result = await ExecuteAsync("Create_Permission", parameters);
            if (result <= 0) return null;
            var newId = parameters.Get<long>("@newID");
            return new PermissionViewModel
            {
                Id = newId,
                RoleId = roleId,
                Function = model.Function,
                Command = model.Command
            };
        }

        public Task DeletePermission(string roleId, string function, string command)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            parameters.Add("@function", function, DbType.String);
            parameters.Add("@command", command, DbType.String);
            return ExecuteAsync("Delete_Permission", parameters);
        }

        public Task UpdatePermissionsByRoleId(string roleId, IEnumerable<PermissioAddModel> permissionCollection)
        {
            var dt = new DataTable();
            dt.Columns.Add("RoleId", typeof(string));
            dt.Columns.Add("Function", typeof(string));
            dt.Columns.Add("Command", typeof(string));
            foreach (var item in permissionCollection)
            {
                dt.Rows.Add(roleId, item.Function, item.Command);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            parameters.Add("@permissions", dt.AsTableValuedParameter("dbo.Permission"));
            return ExecuteAsync("Update_Permission_ByRole", parameters);
        }

        public async Task<IEnumerable<PermissionUserViewModel>> GetPermissionsByUser(User user)
        {
            //current user roles: admin, customer, visitor
            var currentUserRoles = await _userManager.GetRolesAsync(user);
            //load all permissions (vd 30 permission)
            var query = FindAll()
                .Where(x => currentUserRoles.Contains(x.RoleId))
                .Select(x => new Permission(x.Function, x.Command, x.RoleId));

            var result = _mapper.Map<IEnumerable<PermissionUserViewModel>>(query);
            return result;
        }
    }
}