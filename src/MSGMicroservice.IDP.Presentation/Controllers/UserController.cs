using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(IUserRepository userRepository
            )
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RegisterRequestDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register(RegisterRequestDTO obj)
        {
            var result = await _userRepository.Register(obj);
            return result != null ? Ok(result) : NoContent();
        }
    }
}