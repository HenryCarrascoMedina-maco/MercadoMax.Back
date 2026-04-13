using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercadoMAX.Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _roleService.ListAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Roles:Read")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id}/permissions")]
    [Authorize(Policy = "Roles:Read")]
    public async Task<IActionResult> GetPermissions(int id)
    {
        var result = await _roleService.GetPermissionsByRoleAsync(id);
        return Ok(result);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> ListAllPermissions()
    {
        var result = await _roleService.ListAllPermissionsAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Roles:Create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        var result = await _roleService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Roles:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateRoleRequest request)
    {
        var result = await _roleService.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Roles:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _roleService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("permissions")]
    [Authorize(Policy = "Permissions:Create")]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
    {
        var result = await _roleService.CreatePermissionAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("permissions")]
    [Authorize(Policy = "Permissions:Update")]
    public async Task<IActionResult> UpdatePermission([FromBody] UpdatePermissionRequest request)
    {
        var result = await _roleService.UpdatePermissionAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("permissions/{id}")]
    [Authorize(Policy = "Permissions:Delete")]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var result = await _roleService.DeletePermissionAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Roles:Delete")]
    public async Task<IActionResult> ToggleStatusRole(int id)
    {
        var result = await _roleService.ToggleStatusRoleAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("permissions/{id}/toggle-status")]
    [Authorize(Policy = "Permissions:Delete")]
    public async Task<IActionResult> ToggleStatusPermission(int id)
    {
        var result = await _roleService.ToggleStatusPermissionAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
