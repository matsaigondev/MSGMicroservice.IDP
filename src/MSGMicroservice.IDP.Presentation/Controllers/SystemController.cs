using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSGMicroservice.IDP.Infrastructure.Common;
using MSGMicroservice.IDP.Infrastructure.Repositories;
using MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces;
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
        private readonly ISystemRepository _systemRepository;

        public SystemController(IRepositoryManager repository, IPermissionRepository permissionRepository, ISystemRepository systemRepository)
        {
            _repository = repository;
            _permissionRepository = permissionRepository;
            _systemRepository = systemRepository;
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
                new PermissionDTO {Function = Modules.ABR_hrm.ToString()},
                new PermissionDTO {Function = Modules.ABR_tinh.ToString()},
                new PermissionDTO {Function = Modules.ABR_xetduyet.ToString()},
                new PermissionDTO {Function = Modules.ABR_xacnhan.ToString()},
                new PermissionDTO {Function = Modules.ABR_doituong.ToString()},
                new PermissionDTO {Function = Modules.ABR_ngaycong.ToString()},
                new PermissionDTO {Function = Modules.ABR_master.ToString()},
                new PermissionDTO {Function = Modules.ABR_danhmuc.ToString()},
                new PermissionDTO {Function = Modules.ABR_report.ToString()},
                new PermissionDTO {Function = Modules.REPORT.ToString()},
                new PermissionDTO {Function = Modules.CRM.ToString()},
                new PermissionDTO {Function = Modules.CRM_report.ToString()},
                new PermissionDTO {Function = Modules.CRM_exam.ToString()},
                new PermissionDTO {Function = Modules.CRM_customer.ToString()},
                new PermissionDTO {Function = Modules.CRM_lead.ToString()},
                new PermissionDTO {Function = Modules.CRM_lead_import.ToString()},
                new PermissionDTO {Function = Modules.CRM_cdr.ToString()},
                new PermissionDTO {Function = Modules.CRM_library.ToString()},
                new PermissionDTO {Function = Modules.CRM_lead_client.ToString()},
                new PermissionDTO {Function = Modules.CRM_phaco.ToString()},
                new PermissionDTO {Function = Modules.CRM_event.ToString()},
                new PermissionDTO {Function = Modules.HIS.ToString()},
                new PermissionDTO {Function = Modules.NPS.ToString()},
                new PermissionDTO {Function = Modules.HRM.ToString()},
                new PermissionDTO {Function = Modules.HRM_timesheets.ToString()},
                new PermissionDTO {Function = Modules.HRM_employee.ToString()},
                new PermissionDTO {Function = Modules.FIS.ToString()},
                new PermissionDTO {Function = Modules.FIS_report.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_abr.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_crm.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_fis.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_his.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_hrm.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_nps.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_hub.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_115.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_cam.ToString()},
                new PermissionDTO {Function = Modules.Wiki.ToString()},
                new PermissionDTO {Function = Modules.ELEARNING.ToString()},
                new PermissionDTO {Function = Modules.Dashboard_elearning.ToString()},
                new PermissionDTO {Function = Modules.el_event.ToString()},
                new PermissionDTO {Function = Modules.el_dethi.ToString()},
                new PermissionDTO {Function = Modules.el_question.ToString()},
                new PermissionDTO {Function = Modules.el_danhmuc.ToString()},
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

        [HttpGet("getLogsByPaging")]
        [ProducesResponseType(typeof(PermissionDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLogsByPaging([FromQuery] GetCommonPaging model)
        {
            var result = await _systemRepository.GetLogsByPaging(model);
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

        [HttpPost("updatePermissions")]
        public async Task<ActionResult> UpdatePermissions([FromBody] List<PermissionRoleVm> models)
        {
            foreach (var model in models)
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
            }
            return Ok();
        }
    }
}