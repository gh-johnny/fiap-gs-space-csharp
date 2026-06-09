using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrbitalGuardian.API.Settings;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.EventHandlers;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Services;
using OrbitalGuardian.Infrastructure.Auth;
using OrbitalGuardian.Infrastructure.Dispatchers;
using OrbitalGuardian.Infrastructure.Gateways;
using OrbitalGuardian.Infrastructure.Persistence;
using OrbitalGuardian.Infrastructure.Repositories;

namespace OrbitalGuardian.API.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings(configuration);
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddDomainServices();
        services.AddDispatchers();
        services.AddCommandHandlers();
        services.AddQueryHandlers();
        services.AddDomainEventHandlers();
        services.AddAuthServices();
        services.AddGateway(configuration);
        services.AddTransient<DatabaseSeeder>();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();
        services.AddCorsPolicy();
        services.AddRequestTimeoutsPolicy();
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }

    private static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OrbitalGuardianSettings>(configuration.GetSection("OrbitalGuardian"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<OrbitalGuardianDbContext>(options =>
            options.UseSqlite(connectionString));
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISpaceObjectRepository, SpaceObjectRepository>();
        services.AddScoped<IConjunctionEventRepository, ConjunctionEventRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
        return services;
    }

    private static IServiceCollection AddDispatchers(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateSatelliteCommand, SpaceObjectResponse>, CreateSatelliteCommandHandler>();
        services.AddScoped<ICommandHandler<CreateDebrisCommand, SpaceObjectResponse>, CreateDebrisCommandHandler>();
        services.AddScoped<ICommandHandler<CreateSpaceStationCommand, SpaceObjectResponse>, CreateSpaceStationCommandHandler>();
        services.AddScoped<ICommandHandler<AddTelemetryCommand, TelemetryReadingResponse>, AddTelemetryCommandHandler>();
        services.AddScoped<ICommandHandler<DetectConjunctionCommand, ConjunctionEventResponse>, DetectConjunctionCommandHandler>();
        services.AddScoped<ICommandHandler<AcknowledgeAlertCommand, AlertResponse>, AcknowledgeAlertCommandHandler>();
        services.AddScoped<ICommandHandler<RegisterCommand, UserResponse>, RegisterCommandHandler>();
        services.AddScoped<ICommandHandler<LoginCommand, AuthTokenResponse>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UserResponse>, UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<ImportTleDataCommand, IReadOnlyList<SpaceObjectResponse>>, ImportTleDataCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteSpaceObjectCommand, bool>, DeleteSpaceObjectCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteConjunctionEventCommand, bool>, DeleteConjunctionEventCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, bool>, DeleteUserCommandHandler>();
        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetSpaceObjectsQuery, IReadOnlyList<SpaceObjectResponse>>, GetSpaceObjectsQueryHandler>();
        services.AddScoped<IQueryHandler<GetSpaceObjectByIdQuery, SpaceObjectResponse>, GetSpaceObjectByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetConjunctionsQuery, IReadOnlyList<ConjunctionEventResponse>>, GetConjunctionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetActiveConjunctionsQuery, IReadOnlyList<ConjunctionEventResponse>>, GetActiveConjunctionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetConjunctionByIdQuery, ConjunctionEventResponse>, GetConjunctionByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>>, GetUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserResponse>, GetUserByIdQueryHandler>();
        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<ConjunctionDetectedEvent>, ConjunctionDetectedHandler>();
        services.AddScoped<IDomainEventHandler<AlertRaisedEvent>, AlertRaisedHandler>();
        services.AddScoped<IDomainEventHandler<AlertAcknowledgedEvent>, AlertAcknowledgedHandler>();
        services.AddScoped<IDomainEventHandler<TelemetryRecordedEvent>, TelemetryRecordedHandler>();
        return services;
    }

    private static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        return services;
    }

    private static IServiceCollection AddGateway(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (bool.TryParse(configuration["Gateway:UseMock"], out var useMock) && useMock)
            services.AddSingleton<ITleDataGateway, MockTleDataGateway>();
        else
            services.AddScoped<ITleDataGateway, SpaceTrackTleGateway>();
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        return services;
    }

    private static IServiceCollection AddRequestTimeoutsPolicy(this IServiceCollection services)
    {
        services.AddRequestTimeouts(options =>
        {
            options.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        });
        return services;
    }
}
