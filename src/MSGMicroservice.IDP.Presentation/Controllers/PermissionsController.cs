using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSGMicroservice.IDP.Infrastructure.Repositories;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Presentation.Controllers;

[ApiController]
[Route("/api/[controller]/roles/{roleId}")]
[Authorize("Bearer")]
public class PermissionsController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    
    public PermissionsController(IRepositoryManager repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions(string roleId)
    {
        var result = await _repository.Permission.GetPermissionsByRole(roleId);
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreatePermission(string roleId, [FromBody] PermissioAddModel model)
    {
        var result = await _repository.Permission.CreatePermission(roleId, model);
        return result != null ? Ok(result) : NoContent();
        //return CreatedAtRoute("GetById", new { result.Id });//should be used
    }
    
    [HttpDelete("function/{function}/command/{command}")]
    [ProducesResponseType(typeof(PermissionViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePermission(string roleId, [Required] string function, [Required] string command)
    {
        await _repository.Permission.DeletePermission(roleId, function, command);
        return NoContent();
        //return CreatedAtRoute("GetById", new { result.Id });//should be used
    }
    
    [HttpPost("update-permissions")]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdatePermission(string roleId, [FromBody]IEnumerable<PermissioAddModel> permissios)
    {
        await _repository.Permission.UpdatePermissionsByRoleId(roleId, permissios);
        return NoContent();
        //return CreatedAtRoute("GetById", new { result.Id });//should be used
    }
}