using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        
        public async Task<IReadOnlyList<PermissionRoleVm>> GetPermissionRolesByRole(string roleId)
        {
            List<PermissionRoleVm> addList = new List<PermissionRoleVm>();
            var getlistRole = await GetPermissionsByRoleId(roleId);
            foreach (var i in getlistRole)
            {
                PermissionRoleVm addNew = new PermissionRoleVm();
                addNew.Function = i.Function;
                var parameters = new DynamicParameters();
                parameters.Add("@roleId", roleId);
                parameters.Add("@funcId", i.Function);
                var getPermissions = await QueryAsync<Permission>("Get_PermissionRoles_ByRoleId", parameters);
                addNew.View = getPermissions.Where(x => x.Command.Contains("VIEW")).Count()>0 ? true : false;
                addNew.Create = getPermissions.Where(x => x.Command.Contains("CREATE")).Count()>0 ? true : false;
                addNew.Update = getPermissions.Where(x => x.Command.Contains("UPDATE")).Count()>0 ? true : false;
                addNew.Delete = getPermissions.Where(x => x.Command.Contains("DELETE")).Count()>0 ? true : false;
                addNew.Import = getPermissions.Where(x => x.Command.Contains("IMPORT")).Count()>0 ? true : false;
                addNew.Export = getPermissions.Where(x => x.Command.Contains("EXPORT")).Count()>0 ? true : false;
                addNew.Show = getPermissions.Where(x => x.Command.Contains("SHOW")).Count()>0 ? true : false;
                addNew.Print = getPermissions.Where(x => x.Command.Contains("PRINT")).Count()>0 ? true : false;
                addList.Add(addNew);
            }
            return addList;
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

        public Task DeletePermissionById(string roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            return ExecuteAsync("Delete_Permission_ByRoleId", parameters);
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

        public async Task<IReadOnlyList<PermissionDTO>> GetPermissions()
        {
            var parameters = new DynamicParameters();
            var query = await QueryAsync<Permission>("Get_Permission_Grouped", parameters);
            //var result = _mapper.Map<List<PermissionDTO>>(query);
            List<PermissionDTO> addList = new List<PermissionDTO>();
            foreach (var item in query)
            {
                PermissionDTO addNew = new PermissionDTO()
                {
                    Function = item.Function
                };
                addList.Add(addNew);
            }
            return addList;
        }

        public async Task<IReadOnlyList<PermissionDTO>> GetPermissionsByRoleId(string roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            var query = await QueryAsync<Permission>("Get_PermissionGrouped_ByRoleId", parameters);
            //var result = _mapper.Map<List<PermissionDTO>>(query);
            List<PermissionDTO> addList = new List<PermissionDTO>();
            foreach (var item in query)
            {
                PermissionDTO addNew = new PermissionDTO()
                {
                    Function = item.Function
                };
                addList.Add(addNew);
            }
            return addList;
        }

        public async Task<List<string>> GetPermissionsByRoleId0(string roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@roleId", roleId, DbType.String);
            var query = await QueryAsync<Permission>("Get_PermissionGrouped_ByRoleId", parameters);
            //var result = _mapper.Map<List<PermissionDTO>>(query);
            List<string> addList = new List<string>();
            foreach (var item in query)
            {
                addList.Add(item.Function);
            }
            return addList;
        }
    }
}