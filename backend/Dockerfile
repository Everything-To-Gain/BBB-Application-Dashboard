FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Set environment variables to avoid Windows-specific paths
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV DOTNET_CLI_TELEMETRY_OPTOUT=true

# Copy everything and restore/build
COPY . .

# Remove any Windows-specific NuGet cache files
RUN find . -name "*.nuget.dgspec.json" -delete
RUN find . -name "*.nuget.g.props" -delete  
RUN find . -name "*.nuget.g.targets" -delete
RUN find . -name "project.assets.json" -delete
RUN find . -name "project.nuget.cache" -delete

# Restore and build
RUN dotnet restore BBB-Application-Dashboard.slnx --force
WORKDIR /src/Src/BBB-ApplicationDashboard.Api
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Smaller globalisation footprint (matches project InvariantGlobalization)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8085

ENTRYPOINT ["dotnet", "BBB-ApplicationDashboard.Api.dll"]
