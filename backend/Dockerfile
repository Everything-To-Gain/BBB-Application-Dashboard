FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Avoid Windows-specific NuGet fallback paths and speed up restores
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    NUGET_PACKAGES=/root/.nuget/packages

# Copy solution and project files for better layer caching (paths are relative to backend/ context)
COPY BBB-Application-Dashboard.slnx ./
COPY Src/BBB-ApplicationDashboard.Api/BBB-ApplicationDashboard.Api.csproj ./Src/BBB-ApplicationDashboard.Api/
COPY Src/BBB-ApplicationDashboard.Application/BBB-ApplicationDashboard.Application.csproj ./Src/BBB-ApplicationDashboard.Application/
COPY Src/BBB-ApplicationDashboard.Domain/BBB-ApplicationDashboard.Domain.csproj ./Src/BBB-ApplicationDashboard.Domain/
COPY Src/BBB-ApplicationDashboard.Infrastructure/BBB-ApplicationDashboard.Infrastructure.csproj ./Src/BBB-ApplicationDashboard.Infrastructure/

# Restore
RUN dotnet restore BBB-Application-Dashboard.slnx -p:DisableImplicitNuGetFallbackFolder=true

# Copy full backend source
COPY . .

# Remove any pre-existing Windows-built obj/bin to avoid VS fallback paths
RUN find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +

# Restore again after copying full source to regenerate assets for Linux
RUN dotnet restore BBB-Application-Dashboard.slnx -p:DisableImplicitNuGetFallbackFolder=true

# Publish API
WORKDIR /src/Src/BBB-ApplicationDashboard.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false -p:DisableImplicitNuGetFallbackFolder=true

# RUNTIME STAGE
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Smaller globalization footprint to match project setting
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BBB-ApplicationDashboard.Api.dll"]
