using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Auth.API.Services;

public interface IRoleService
{
    Task<ApiResponse<List<RoleResponse>>> ListAsync();
    Task<ApiResponse<RoleResponse>> GetByIdAsync(int id);
    Task<ApiResponse<List<PermissionResponse>>> GetPermissionsByRoleAsync(int roleId);
    Task<ApiResponse<List<PermissionResponse>>> ListAllPermissionsAsync();
    Task<ApiResponse<int>> CreateAsync(CreateRoleRequest request);
    Task<ApiResponse<string>> UpdateAsync(UpdateRoleRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<int>> CreatePermissionAsync(CreatePermissionRequest request);
    Task<ApiResponse<string>> UpdatePermissionAsync(UpdatePermissionRequest request);
    Task<ApiResponse<string>> DeletePermissionAsync(int id);
    Task<ApiResponse<string>> ToggleStatusRoleAsync(int id);
    Task<ApiResponse<string>> ToggleStatusPermissionAsync(int id);
}

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepo;

    public RoleService(IRoleRepository roleRepo)
    {
        _roleRepo = roleRepo;
    }

    public async Task<ApiResponse<List<RoleResponse>>> ListAsync()
    {
        var roles = await _roleRepo.ListAsync();
        return ApiResponse<List<RoleResponse>>.Ok(roles);
    }

    public async Task<ApiResponse<RoleResponse>> GetByIdAsync(int id)
    {
        var role = await _roleRepo.GetByIdAsync(id);
        if (role == null)
            return ApiResponse<RoleResponse>.Fail("Role not found");

        return ApiResponse<RoleResponse>.Ok(role);
    }

    public async Task<ApiResponse<List<PermissionResponse>>> GetPermissionsByRoleAsync(int roleId)
    {
        var permissions = await _roleRepo.GetPermissionsByRoleAsync(roleId);
        return ApiResponse<List<PermissionResponse>>.Ok(permissions);
    }

    public async Task<ApiResponse<List<PermissionResponse>>> ListAllPermissionsAsync()
    {
        var permissions = await _roleRepo.ListAllPermissionsAsync();
        return ApiResponse<List<PermissionResponse>>.Ok(permissions);
    }

    public async Task<ApiResponse<int>> CreateAsync(CreateRoleRequest request)
    {
        var permIds = string.Join(",", request.PermissionIds);
        var result = await _roleRepo.CreateAsync(request.Name, request.Description, permIds);
        return result.Success == 1
            ? ApiResponse<int>.Ok(result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateRoleRequest request)
    {
        var permIds = string.Join(",", request.PermissionIds);
        var result = await _roleRepo.UpdateAsync(request.Id, request.Name, request.Description, permIds);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var result = await _roleRepo.DeleteAsync(id);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<int>> CreatePermissionAsync(CreatePermissionRequest request)
    {
        var result = await _roleRepo.CreatePermissionAsync(request.Module, request.Action, request.Description);
        return result.Success == 1
            ? ApiResponse<int>.Ok(result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> UpdatePermissionAsync(UpdatePermissionRequest request)
    {
        var result = await _roleRepo.UpdatePermissionAsync(request.Id, request.Module, request.Action, request.Description);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> DeletePermissionAsync(int id)
    {
        var result = await _roleRepo.DeletePermissionAsync(id);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusRoleAsync(int id)
    {
        var result = await _roleRepo.ToggleStatusRoleAsync(id);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusPermissionAsync(int id)
    {
        var result = await _roleRepo.ToggleStatusPermissionAsync(id);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }
}
