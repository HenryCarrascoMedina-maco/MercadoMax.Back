using MercadoMAX.Auth.API.Configuration;
using MercadoMAX.Auth.API.Repositories;
using MercadoMAX.Auth.API.Services;
using MercadoMAX.Shared.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using MercadoMAX.Shared.Authorization;
using MercadoMAX.Shared.CrossCutting.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ── Connection string ───────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not configured");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

// ── JWT Settings ────────────────────────────────────────
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            try
            {
                var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);
                var tokenVersionClaim = context.Principal?.FindFirst("tokenVersion");

                // Tokens sin tokenVersion (sesiones antiguas) → se dejan pasar sin validar versión
                if (userIdClaim is null || tokenVersionClaim is null) return;

                if (!int.TryParse(userIdClaim.Value, out var userId) ||
                    !int.TryParse(tokenVersionClaim.Value, out var jwtVersion)) return;

                var authRepo = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
                var dbVersion = await authRepo.GetTokenVersionAsync(userId);
                if (dbVersion != jwtVersion)
                    context.Fail("Token has been invalidated");
            }
            catch
            {
                // Si la columna aún no existe en BD, se permite el paso sin validar versión
            }
        }
    };
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddAuthorization();

// ── Repositories ────────────────────────────────────────
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// ── Services ────────────────────────────────────────────
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// ── Controllers + Swagger ───────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MercadoMAX Auth API", Version = "v1" });
    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// ── CORS ────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddHealthChecks();

// ── Cross-cutting (Fase 0: aditivo, no toca controllers/services/repos/SPs) ──
builder.Services.AddMercadoMaxCrossCutting(builder.Configuration);

var app = builder.Build();

// ── Pipeline ────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseMercadoMaxCrossCutting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
