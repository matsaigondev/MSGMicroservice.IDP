using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSGMicroservice.IDP.Infrastructure.Repositories;
using MSGMicroservice.IDP.Infrastructure.ViewModels;
using MSGMicroservice.IDP.Presentation.Common;

namespace MSGMicroservice.IDP.Presentation.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize("Bearer")]
    public class SystemController: ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly IPermissionRepository _permissionRepository;

        public SystemController(IRepositoryManager repository, IPermissionRepository permissionRepository)
        {
            _repository = repository;
            _permissionRepository = permissionRepository;
        }
        
        [HttpGet("getallpermissions")]
        [ProducesResponseType(typeof(PermissionDTO), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetPermissionsGrouped()
        {
            var result = await _repository.Permission.GetPermissions();
            return Ok(result);
        }
        
        [HttpGet("getcatpermissions")]
        [ProducesResponseType(typeof(PermissionDTO), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetCatPermissions()
        {
            List<PermissionDTO> _addList = new List<PermissionDTO>();
            _addList.AddRange(new List<PermissionDTO>
            {
                new PermissionDTO {Function = Modules.ROLE.ToString()},
                new PermissionDTO {Function = Modules.USER.ToString()},
                new PermissionDTO {Function = Modules.PATIENT.ToString()},
                new PermissionDTO {Function = Modules.PROJECT.ToString()},
                new PermissionDTO {Function = Modules.PERMISSION.ToString()},
                new PermissionDTO {Function = Modules.CUSTOMER.ToString()},
                new PermissionDTO {Function = Modules.SETTING.ToString()},
                new PermissionDTO {Function = Modules.ABR.ToString()},
                new PermissionDTO {Function = Modules.REPORT.ToString()},
                new PermissionDTO {Function = Modules.CRM.ToString()},
                new PermissionDTO {Function = Modules.HIS.ToString()},
                new PermissionDTO {Function = Modules.NPS.ToString()},
            });
            return Ok(_addList.ToList().OrderBy(x => x.Function));
        }
        
        [HttpGet("getlistpermissionsbyid/{id}")]
        [ProducesResponseType(typeof(PermissionRoleVm), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetListPermissionsById(string id)
        {
            var result = await _permissionRepository.GetPermissionRolesByRole(id);
            return Ok(result);
        }
        
        [HttpPost("updatePermission")]
        public async Task<ActionResult> UpdatePermission([FromBody] PermissionRoleVm model)
        {
            if (model.View)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "VIEW"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "VIEW");
            }
            if (model.Create)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "CREATE"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "CREATE");
            }
            if (model.Update)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "UPDATE"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "UPDATE");
            }
            if (model.Delete)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "DELETE"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "DELETE");
            }
            if (model.Import)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "IMPORT"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "IMPORT");
            }
            if (model.Export)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "EXPORT"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "EXPORT");
            }
            if (model.Show)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "SHOW"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "SHOW");
            }
            if (model.Print)
            {
                PermissioAddModel addModel = new PermissioAddModel()
                {
                    Function = model.Function,
                    Command = "PRINT"
                };
                await _permissionRepository.CreatePermission(model.RoleId, addModel);
            }
            else
            {
                await _permissionRepository.DeletePermission(model.RoleId, model.Function, "PRINT");
            }
            return Ok();
        }
    }
}