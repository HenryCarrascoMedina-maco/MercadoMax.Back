using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Repositories;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Auth.API.Services;

public interface IUserService
{
    Task<ApiResponse<int>> CreateAsync(CreateUserRequest request);
    Task<ApiResponse<UserResponse>> GetByIdAsync(int id);
    Task<PagedResponse<UserResponse>> ListAsync(string? search, int pageNumber, int pageSize);
    Task<ApiResponse<string>> UpdateAsync(UpdateUserRequest request);
    Task<ApiResponse<string>> DeleteAsync(int id);
    Task<ApiResponse<string>> AssignRoleAsync(AssignRoleRequest request);
    Task<ApiResponse<string>> RemoveRoleAsync(int userId, int roleId);
    Task<ApiResponse<string>> AssignPermissionAsync(AssignPermissionRequest request);
    Task<ApiResponse<string>> RemovePermissionAsync(int userId, int permissionId);
    Task<ApiResponse<List<int>>> GetDirectPermissionIdsAsync(int userId);
    Task<ApiResponse<string>> AssignRolePermissionsAsync(AssignRoleRequest request);
    Task<ApiResponse<string>> ToggleStatusAsync(int id);
    Task<ApiResponse<string>> RevokeSessionAsync(int userId);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthRepository _authRepo;

    public UserService(IUserRepository userRepo, IAuthRepository authRepo)
    {
        _userRepo = userRepo;
        _authRepo = authRepo;
    }

    public async Task<ApiResponse<int>> CreateAsync(CreateUserRequest request)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var result = await _userRepo.CreateAsync(request.FirstName, request.LastName, request.Email, passwordHash, request.Phone);

        return result.Success == 1
            ? ApiResponse<int>.Ok(result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<UserResponse>> GetByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return ApiResponse<UserResponse>.Fail("User not found");

        var roles = await _authRepo.GetUserRolesAsync(id);

        return ApiResponse<UserResponse>.Ok(new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            Roles = roles
        });
    }

    public async Task<PagedResponse<UserResponse>> ListAsync(string? search, int pageNumber, int pageSize)
    {
        var (rows, totalRecords) = await _userRepo.ListAsync(search, pageNumber, pageSize);

        var items = rows.Select(r => new UserResponse
        {
            Id = r.Id,
            FirstName = r.FirstName,
            LastName = r.LastName,
            Email = r.Email,
            Phone = r.Phone,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            Roles = string.IsNullOrEmpty(r.RoleName) ? [] : r.RoleName.Split(',', StringSplitOptions.TrimEntries).ToList()
        }).ToList();

        return new PagedResponse<UserResponse>
        {
            Data = items,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponse<string>> UpdateAsync(UpdateUserRequest request)
    {
        var result = await _userRepo.UpdateAsync(request.Id, request.FirstName, request.LastName, request.Email, request.Phone);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> DeleteAsync(int id)
    {
        var result = await _userRepo.DeleteAsync(id);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> AssignRoleAsync(AssignRoleRequest request)
    {
        var result = await _userRepo.AssignRoleAsync(request.UserId, request.RoleId);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> RemoveRoleAsync(int userId, int roleId)
    {
        var result = await _userRepo.RemoveRoleAsync(userId, roleId);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> AssignPermissionAsync(AssignPermissionRequest request)
    {
        var result = await _userRepo.AssignPermissionAsync(request.UserId, request.PermissionId);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> RemovePermissionAsync(int userId, int permissionId)
    {
        var result = await _userRepo.RemovePermissionAsync(userId, permissionId);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<List<int>>> GetDirectPermissionIdsAsync(int userId)
    {
        var ids = await _userRepo.GetDirectPermissionIdsAsync(userId);
        return ApiResponse<List<int>>.Ok(ids);
    }

    public async Task<ApiResponse<string>> AssignRolePermissionsAsync(AssignRoleRequest request)
    {
        var result = await _userRepo.AssignRolePermissionsAsync(request.UserId, request.RoleId);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> ToggleStatusAsync(int id)
    {
        var result = await _userRepo.ToggleStatusAsync(id);
        return result.Success == 1
            ? ApiResponse<string>.Ok(result.Message)
            : ApiResponse<string>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> RevokeSessionAsync(int userId)
    {
        await _authRepo.IncrementTokenVersionAsync(userId);
        return ApiResponse<string>.Ok("Session revoked");
    }
}
