using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrbitalGuardian.API.Middleware;
using OrbitalGuardian.API.Settings;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.EventHandlers;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Services;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Infrastructure.Auth;
using OrbitalGuardian.Infrastructure.Dispatchers;
using OrbitalGuardian.Infrastructure.Gateways;
using OrbitalGuardian.Infrastructure.Persistence;
using OrbitalGuardian.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ── Settings ─────────────────────────────────────────────────────────────────
builder.Services.Configure<OrbitalGuardianSettings>(
    configuration.GetSection("OrbitalGuardian"));
builder.Services.Configure<JwtSettings>(
    configuration.GetSection("Jwt"));

// ── DbContext ─────────────────────────────────────────────────────────────────
var connectionString = configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<OrbitalGuardianDbContext>(options =>
    options.UseSqlite(connectionString));

// ── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ISpaceObjectRepository, SpaceObjectRepository>();
builder.Services.AddScoped<IConjunctionEventRepository, ConjunctionEventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ── Domain Services ───────────────────────────────────────────────────────────
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();

// ── Dispatchers ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// ── Command Handlers ──────────────────────────────────────────────────────────
builder.Services.AddScoped<ICommandHandler<CreateSatelliteCommand, OrbitalGuardian.Application.DTOs.SpaceObjectResponse>, CreateSatelliteCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateDebrisCommand, OrbitalGuardian.Application.DTOs.SpaceObjectResponse>, CreateDebrisCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreateSpaceStationCommand, OrbitalGuardian.Application.DTOs.SpaceObjectResponse>, CreateSpaceStationCommandHandler>();
builder.Services.AddScoped<ICommandHandler<AddTelemetryCommand, OrbitalGuardian.Application.DTOs.TelemetryReadingResponse>, AddTelemetryCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DetectConjunctionCommand, OrbitalGuardian.Application.DTOs.ConjunctionEventResponse>, DetectConjunctionCommandHandler>();
builder.Services.AddScoped<ICommandHandler<AcknowledgeAlertCommand, OrbitalGuardian.Application.DTOs.AlertResponse>, AcknowledgeAlertCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RegisterCommand, OrbitalGuardian.Application.DTOs.UserResponse>, RegisterCommandHandler>();
builder.Services.AddScoped<ICommandHandler<LoginCommand, OrbitalGuardian.Application.DTOs.AuthTokenResponse>, LoginCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateUserCommand, OrbitalGuardian.Application.DTOs.UserResponse>, UpdateUserCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ImportTleDataCommand, System.Collections.Generic.IReadOnlyList<OrbitalGuardian.Application.DTOs.SpaceObjectResponse>>, ImportTleDataCommandHandler>();

// ── Query Handlers ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IQueryHandler<GetSpaceObjectsQuery, System.Collections.Generic.IReadOnlyList<OrbitalGuardian.Application.DTOs.SpaceObjectResponse>>, GetSpaceObjectsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetSpaceObjectByIdQuery, OrbitalGuardian.Application.DTOs.SpaceObjectResponse>, GetSpaceObjectByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetConjunctionsQuery, System.Collections.Generic.IReadOnlyList<OrbitalGuardian.Application.DTOs.ConjunctionEventResponse>>, GetConjunctionsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetActiveConjunctionsQuery, System.Collections.Generic.IReadOnlyList<OrbitalGuardian.Application.DTOs.ConjunctionEventResponse>>, GetActiveConjunctionsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetConjunctionByIdQuery, OrbitalGuardian.Application.DTOs.ConjunctionEventResponse>, GetConjunctionByIdQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetUsersQuery, System.Collections.Generic.IReadOnlyList<OrbitalGuardian.Application.DTOs.UserResponse>>, GetUsersQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetUserByIdQuery, OrbitalGuardian.Application.DTOs.UserResponse>, GetUserByIdQueryHandler>();

// ── Domain Event Handlers ─────────────────────────────────────────────────────
builder.Services.AddScoped<IDomainEventHandler<ConjunctionDetectedEvent>, ConjunctionDetectedHandler>();
builder.Services.AddScoped<IDomainEventHandler<AlertRaisedEvent>, AlertRaisedHandler>();
builder.Services.AddScoped<IDomainEventHandler<AlertAcknowledgedEvent>, AlertAcknowledgedHandler>();
builder.Services.AddScoped<IDomainEventHandler<TelemetryRecordedEvent>, TelemetryRecordedHandler>();

// ── Auth Services ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

// ── Gateway ───────────────────────────────────────────────────────────────────
if (configuration["Gateway:UseMock"] == "true")
    builder.Services.AddSingleton<ITleDataGateway, MockTleDataGateway>();
else
    builder.Services.AddScoped<ITleDataGateway, SpaceTrackTleGateway>();

// ── DatabaseSeeder ────────────────────────────────────────────────────────────
builder.Services.AddTransient<DatabaseSeeder>();

// ── Authentication ────────────────────────────────────────────────────────────
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ── RequestTimeout ────────────────────────────────────────────────────────────
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(30)
    };
});

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orbital Guardian API",
        Version = "v1"
    });

    if (System.IO.File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

OrbitalLogger.Initialize(app.Services.GetRequiredService<ILoggerFactory>());

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseRequestTimeouts();
app.MapControllers();

// ── Seed ──────────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.Run();
