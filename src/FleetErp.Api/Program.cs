using System.Text;
using System.Text.Json.Serialization;
using FleetErp.Api.Controllers;
using FleetErp.Api.Middleware;
using FleetErp.Application;
using FleetErp.Application.Abstractions;
using FleetErp.Domain.Enums;
using FleetErp.Infrastructure;
using FleetErp.Infrastructure.Identity;
using FleetErp.Infrastructure.Persistence;
using FleetErp.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Composición de capas -------------------------------------------------
// El host solo conoce el módulo de cada capa, no sus clases concretas.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserAccessor>();

builder.Services
    .AddControllers(options => options.Filters.Add<FluentValidationFilter>())
    .AddJsonOptions(options =>
    {
        // Los enumerados viajan como texto: el portal y cualquier integrador leen
        // "InProgress" en lugar de un 1 que nadie puede interpretar.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FleetERP · API de control de flotilla",
        Version = "v1",
        Description = "Servicios de inventario de unidades, operadores, viajes, combustible, gastos y tableros."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token obtenido en /api/auth/login."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
    });

    var xml = Path.Combine(AppContext.BaseDirectory,
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) options.IncludeXmlComments(xml);
});

// ---- Autenticación y autorización ----------------------------------------
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwt.SigningKey))
    throw new InvalidOperationException("Configure Jwt:SigningKey antes de iniciar la aplicación.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = jwt.CreateSigningKey(),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.CanWrite, policy =>
        policy.RequireRole(nameof(UserRole.Administrator), nameof(UserRole.Dispatcher)))
    .AddPolicy(Policies.IsAdministrator, policy =>
        policy.RequireRole(nameof(UserRole.Administrator)));

// El portal corre en su propio origen y su propio despliegue: sin CORS explícito
// no podría consumir esta API.
const string PortalCors = "portal";
builder.Services.AddCors(options => options.AddPolicy(PortalCors, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["https://localhost:7100"])
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddHealthChecks();

var app = builder.Build();

//await InitializeDatabaseAsync(app);

app.UseFleetExceptionHandling();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue("Swagger:Enabled", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetERP API v1");
        options.DocumentTitle = "FleetERP · API";
    });
}

app.UseCors(PortalCors);
app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();

/// <summary>
/// Prepara la base al arrancar: migraciones en PostgreSQL, creación directa del
/// esquema en memoria, y datos de demostración cuando la configuración lo pide.
/// </summary>
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FleetDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    if (InfrastructureModule.ResolveProvider(app.Configuration) == DatabaseProvider.InMemory)
    {
        await context.Database.EnsureCreatedAsync();
    }
    else if (app.Configuration.GetValue("Database:AutoMigrate", true))
    {
        logger.LogInformation("Aplicando migraciones pendientes de PostgreSQL…");
        await context.Database.MigrateAsync();
    }

    if (!app.Configuration.GetValue("Database:SeedDemoData", false)) return;

    await services.GetRequiredService<DemoDataSeeder>().SeedAsync();
}

/// <summary>Punto de extensión para pruebas de integración con <c>WebApplicationFactory</c>.</summary>
public partial class Program;
