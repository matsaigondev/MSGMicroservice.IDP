using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSGMicroservice.IDP.Infrastructure.Common;
using MSGMicroservice.IDP.Infrastructure.Common.ApiResult;
using MSGMicroservice.IDP.Infrastructure.Repositories;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Presentation.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize("Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        protected APIResponse _response;
        
        public UserController(IUserRepository userRepository, IPermissionRepository permissionRepository)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _response = new();
        }

        [HttpPost]
        [ProducesResponseType(typeof(RegisterRequestDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterRequestDTO obj)
        {
            var result = await _userRepository.Register(obj);
            return result != null ? Ok(result) : BadRequest();
        }
        
        [HttpPost("createUser")]
        [ProducesResponseType(typeof(RegisterRequestDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegisterV2(RegisterRequestDTO obj)
        {
            var result = await _userRepository.RegisterV2(obj);
            return result != null ? Ok(result) : BadRequest();
        }

        [HttpPost("dangky")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterRequestDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterRequestDTO obj)
        {
            var result = await _userRepository.DangKyTaiKhoan(obj);
            return result != null ? Ok(result) : BadRequest();
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        //{
        //    var loginResponse = await _userRepository.Login(model);
        //    if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.access_token))
        //    {
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages.Add("Username or password is incorrect");
        //        return BadRequest(_response);
        //    }
        //    _response.StatusCode = HttpStatusCode.OK;
        //    _response.IsSuccess = true;
        //    _response.Result = loginResponse;
        //    return Ok(_response);
        //}

        [HttpDelete("deleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var result = await _userRepository.Delete(id);
            if (!result)
            {
                return NotFound();
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Deleted user successful";
            return Ok(_response);
        }
        
        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetUserPassword([FromBody] RegisterRequestDTO model)
        {
            var result = await _userRepository.ResetPassword(model);
            if (!result)
            {
                return NotFound();
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Reset password successful";
            return Ok(_response);
        }

        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePwdDto model)
        {
            var result = await _userRepository.ChangePassword(model);
            if (!result)
            {
                return NotFound();
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Changed password successful";
            return Ok(_response);
        }
        
        [HttpPut("updateUser")]
        public async Task<ActionResult> UpdateUser([FromBody] RegisterRequestDTO model)
        {
            var result = await _userRepository.Update(model);
            if (!result)
            {
                return NotFound();
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Updated user successful";
            return Ok(_response);
        }
        
        [HttpGet("getusers")]
        public async Task<ActionResult> GetUsers()
        {
            var result = await _userRepository.GetUsers();
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = result;
            return Ok(_response);
        }
        
        [HttpGet("getuserspaging")]
        public async Task<ActionResult> GetUsersPaging([FromQuery] GetCommonPaging model)
        {
            var result = await _userRepository.GetUsersPaging(model);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = result;
            return Ok(_response);
        }
        
        [HttpGet("getuser/{id}")]
        public async Task<ActionResult> GetUser(string id)
        {
            var result = await _userRepository.GetUserById(id);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = result;
            return Ok(_response);
        }

        #region Role
        [HttpGet("getroles")]
        public async Task<ActionResult> GetRoles()
        {
            var result = await _userRepository.GetRoles();
            foreach (var item in result)
            {
                var getPermissions = await _permissionRepository.GetPermissionsByRoleId0(item.Id);
                item.Functions = getPermissions;
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = result;
            return Ok(_response);
        }
        
        [HttpPost("createRole")]
        public async Task<ActionResult> CreateRole([FromBody] RoleDTO model)
        {
            var result = await _userRepository.CreateRole(model);
            if (result == null)
            {
                return NotFound();
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Create new successful";
            return Ok(_response);
        }
        
        [HttpPut("updateRole")]
        public async Task<ActionResult> UpdateRole([FromBody] RoleDTO model)
        {
            var result = await _userRepository.UpdateRole(model);
            if (!result)
            {
                return NotFound();
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Updated successful";
            return Ok(_response);
        }
        
        [HttpDelete("deleteRole/{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            var result = await _userRepository.DeleteRole(id);
            if (!result)
            {
                return NotFound();
            }

            await _permissionRepository.DeletePermissionById(id);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Deleted role successful";
            return Ok(_response);
        }
        #endregion Role
    }
}