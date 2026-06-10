FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OrbitalGuardian.sln ./
COPY src/OrbitalGuardian.Domain/OrbitalGuardian.Domain.csproj src/OrbitalGuardian.Domain/
COPY src/OrbitalGuardian.Application/OrbitalGuardian.Application.csproj src/OrbitalGuardian.Application/
COPY src/OrbitalGuardian.Infrastructure/OrbitalGuardian.Infrastructure.csproj src/OrbitalGuardian.Infrastructure/
COPY src/OrbitalGuardian.API/OrbitalGuardian.API.csproj src/OrbitalGuardian.API/
COPY src/OrbitalGuardian.IoC/OrbitalGuardian.IoC.csproj src/OrbitalGuardian.IoC/
COPY tests/OrbitalGuardian.Tests/OrbitalGuardian.Tests.csproj tests/OrbitalGuardian.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/OrbitalGuardian.API -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/data
EXPOSE 8080
ENTRYPOINT ["dotnet", "OrbitalGuardian.API.dll"]
