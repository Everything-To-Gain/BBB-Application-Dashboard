# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Set environment variables for better performance
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    NUGET_PACKAGES=/root/.nuget/packages

# Copy solution and project files for restore
COPY BBB-Application-Dashboard.slnx ./
COPY Src/BBB-ApplicationDashboard.Api/BBB-ApplicationDashboard.Api.csproj ./Src/BBB-ApplicationDashboard.Api/
COPY Src/BBB-ApplicationDashboard.Application/BBB-ApplicationDashboard.Application.csproj ./Src/BBB-ApplicationDashboard.Application/
COPY Src/BBB-ApplicationDashboard.Domain/BBB-ApplicationDashboard.Domain.csproj ./Src/BBB-ApplicationDashboard.Domain/
COPY Src/BBB-ApplicationDashboard.Infrastructure/BBB-ApplicationDashboard.Infrastructure.csproj ./Src/BBB-ApplicationDashboard.Infrastructure/

# Restore dependencies
RUN dotnet restore BBB-Application-Dashboard.slnx

# Copy source code
COPY . .

# Build and publish
WORKDIR /src/Src/BBB-ApplicationDashboard.Api
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8085

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8085

# Start the application
ENTRYPOINT ["dotnet", "BBB-ApplicationDashboard.Api.dll"]