using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSGMicroservice.IDP.Infrastructure.Common.ApiResult;
using MSGMicroservice.IDP.Infrastructure.Repositories;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Presentation.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    // [Authorize("Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        [HttpPost("resetpassword")]
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
    }
}