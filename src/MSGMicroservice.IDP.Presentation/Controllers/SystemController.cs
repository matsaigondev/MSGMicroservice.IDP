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
    }
}