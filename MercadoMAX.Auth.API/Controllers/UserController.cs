using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MercadoMAX.Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "Users:List")]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _userService.ListAsync(search, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Users:Read")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Policy = "Users:Create")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HttpPut]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Users:Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("assign-role")]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var result = await _userService.AssignRoleAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{userId}/role/{roleId}")]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var result = await _userService.RemoveRoleAsync(userId, roleId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("assign-permission")]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionRequest request)
    {
        var result = await _userService.AssignPermissionAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{userId}/permission/{permissionId}")]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> RemovePermission(int userId, int permissionId)
    {
        var result = await _userService.RemovePermissionAsync(userId, permissionId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}/permissions")]
    [Authorize(Policy = "Users:Read")]
    public async Task<IActionResult> GetDirectPermissions(int id)
    {
        var result = await _userService.GetDirectPermissionIdsAsync(id);
        return Ok(result);
    }

    [HttpPost("assign-role-permissions")]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> AssignRolePermissions([FromBody] AssignRoleRequest request)
    {
        var result = await _userService.AssignRolePermissionsAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [Authorize(Policy = "Users:Delete")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _userService.ToggleStatusAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/revoke-session")]
    [Authorize(Policy = "Users:Update")]
    public async Task<IActionResult> RevokeSession(int id)
    {
        var result = await _userService.RevokeSessionAsync(id);
        return Ok(result);
    }
}
