using OrbitalGuardian.API.Extensions;
using OrbitalGuardian.API.Swagger;
using OrbitalGuardian.API.Middleware;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

OrbitalLogger.Initialize(app.Services.GetRequiredService<ILoggerFactory>());

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseRequestTimeouts();
app.MapControllers();

await app.MigrateAndSeedAsync();

app.Run();
