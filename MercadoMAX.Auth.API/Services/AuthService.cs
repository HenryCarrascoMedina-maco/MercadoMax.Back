using MercadoMAX.Auth.API.Configuration;
using MercadoMAX.Auth.API.DTOs;
using MercadoMAX.Auth.API.Repositories;
using MercadoMAX.Shared.DTOs;
using Microsoft.Extensions.Options;

namespace MercadoMAX.Auth.API.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<int>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
}

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepo;
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwt;

    public AuthService(IAuthRepository authRepo, IUserRepository userRepo, ITokenService tokenService, IOptions<JwtSettings> jwt)
    {
        _authRepo = authRepo;
        _userRepo = userRepo;
        _tokenService = tokenService;
        _jwt = jwt.Value;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _authRepo.GetUserByEmailAsync(request.Email);
        if (user == null || user.Status == 0)
            return ApiResponse<LoginResponse>.Fail("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<LoginResponse>.Fail("Invalid credentials");

        var roles = await _authRepo.GetUserRolesAsync(user.Id);
        var permissions = await _authRepo.GetUserPermissionsAsync(user.Id);
        var fullName = $"{user.FirstName} {user.LastName}";

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, fullName, roles, permissions, user.TokenVersion);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays);

        await _authRepo.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshExpiry);

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            UserId = user.Id,
            FullName = fullName,
            Email = user.Email,
            Token = accessToken,
            RefreshToken = refreshToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.TokenExpirationMinutes),
            Roles = roles,
            Permissions = permissions
        }, "Login successful");
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
            return ApiResponse<LoginResponse>.Fail("Invalid token");

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return ApiResponse<LoginResponse>.Fail("Invalid token");

        var user = await _authRepo.GetUserByEmailAsync(
            principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "");

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return ApiResponse<LoginResponse>.Fail("Invalid or expired refresh token");

        var roles = await _authRepo.GetUserRolesAsync(user.Id);
        var permissions = await _authRepo.GetUserPermissionsAsync(user.Id);
        var fullName = $"{user.FirstName} {user.LastName}";

        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, fullName, roles, permissions, user.TokenVersion);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays);

        await _authRepo.UpdateRefreshTokenAsync(user.Id, newRefreshToken, refreshExpiry);

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            UserId = user.Id,
            FullName = fullName,
            Email = user.Email,
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.TokenExpirationMinutes),
            Roles = roles,
            Permissions = permissions
        }, "Token refreshed");
    }

    public async Task<ApiResponse<int>> RegisterAsync(RegisterRequest request)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var result = await _userRepo.RegisterAsync(request.FirstName, request.LastName, request.Email, passwordHash, request.Phone);

        return result.Success == 1
            ? ApiResponse<int>.Ok(result.Id, result.Message)
            : ApiResponse<int>.Fail(result.Message);
    }

    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // Generate a cryptographically secure token
        var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = await _authRepo.CreatePasswordResetTokenAsync(request.Email, token, expiresAt);

        // Always return success to avoid email enumeration
        if (result.Success == 0)
            return ApiResponse<string>.Ok("If the email exists, a reset link has been generated");

        // In production, send email with the token/link here
        // For development, return the token in the response
        return ApiResponse<string>.Ok(token, "Reset token generated. Use POST /api/auth/reset-password with this token.");
    }

    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var validation = await _authRepo.ValidatePasswordResetTokenAsync(request.Token);
        if (validation.Success == 0)
            return ApiResponse<string>.Fail(validation.Message);

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        var result = await _authRepo.ResetPasswordAsync(request.Token, newHash);

        return result.Success == 1
            ? ApiResponse<string>.Ok("Password reset successfully")
            : ApiResponse<string>.Fail(result.Message);
    }
}
