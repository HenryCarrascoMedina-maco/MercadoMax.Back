using System.ComponentModel.DataAnnotations;

namespace MercadoMAX.Auth.API.DTOs;

public class CreateUserRequest
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }
}

public class UpdateUserRequest
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }
}

public class UserResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = [];
}

// Internal DTO mapped by Dapper from SP_LIST_USER (STRING_AGG returns a single string)
public class UserListRow
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

public class RoleResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PermissionResponse
{
    public int Id { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; }
}

public class AssignRoleRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int RoleId { get; set; }
}

public class CreateRoleRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public List<int> PermissionIds { get; set; } = [];
}

public class UpdateRoleRequest
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public List<int> PermissionIds { get; set; } = [];
}

public class CreatePermissionRequest
{
    [Required, MaxLength(50)]
    public string Module { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}

public class UpdatePermissionRequest
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Module { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }
}

public class AssignPermissionRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int PermissionId { get; set; }
}
