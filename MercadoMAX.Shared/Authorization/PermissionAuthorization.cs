using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MercadoMAX.Shared.Authorization;

// ─── Requirement ──────────────────────────────────────────────────────────────
// Represents the specific "Module:Action" string that a user must possess in
// their JWT "permission" claims in order to access an endpoint.
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}

// ─── Handler ──────────────────────────────────────────────────────────────────
// Validates that the authenticated user has a JWT claim of type "permission"
// whose value matches the required permission.
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission = context.User.Claims
            .Any(c => c.Type == "permission" && c.Value == requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

// ─── Policy Provider ──────────────────────────────────────────────────────────
// Dynamically builds an authorization policy for any string in the form
// "Module:Action" (e.g., "Guides:Create", "Masters:Delete").
// This avoids having to register each permission as a named policy at startup.
public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options) { }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check built-in policies first (e.g., existing role-based ones)
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null) return policy;

        // Any "Module:Action" key → create a permission policy on the fly
        if (policyName.Contains(':'))
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }

        return null;
    }
}
