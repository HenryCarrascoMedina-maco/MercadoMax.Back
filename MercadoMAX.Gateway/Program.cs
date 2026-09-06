using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Ocelot configuration ──────────────────────────────────
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// ── CORS ──────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:4200"];

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

// ── Health Checks (configurable per environment) ──────────
var svc = builder.Configuration.GetSection("Services");
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{svc["Auth"]}/health"),         name: "auth-api",         tags: ["backend"])
    .AddUrlGroup(new Uri($"{svc["Maestros"]}/health"),     name: "maestros-api",     tags: ["backend"])
    .AddUrlGroup(new Uri($"{svc["Guias"]}/health"),        name: "guias-api",        tags: ["backend"])
    .AddUrlGroup(new Uri($"{svc["Transporte"]}/health"),   name: "transporte-api",   tags: ["backend"])
    .AddUrlGroup(new Uri($"{svc["Recepcion"]}/health"),    name: "recepcion-api",    tags: ["backend"])
    .AddUrlGroup(new Uri($"{svc["Comerciante"]}/health"),  name: "comerciante-api",  tags: ["backend"])
    .AddUrlGroup(new Uri($"{svc["Finanzas"]}/health"),     name: "finanzas-api",     tags: ["backend"]);

// ── Ocelot ────────────────────────────────────────────────
builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors();
app.UseHealthChecks("/health");

await app.UseOcelot();

app.Run();
