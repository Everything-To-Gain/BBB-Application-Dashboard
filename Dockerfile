# Use the official .NET 9 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /src

# Copy the solution file
COPY BBB-Application-Dashboard.sln ./

# Copy all project files
COPY Src/BBB-ApplicationDashboard.Api/*.csproj ./Src/BBB-ApplicationDashboard.Api/
COPY Src/BBB-ApplicationDashboard.Application/*.csproj ./Src/BBB-ApplicationDashboard.Application/
COPY Src/BBB-ApplicationDashboard.Domain/*.csproj ./Src/BBB-ApplicationDashboard.Domain/
COPY Src/BBB-ApplicationDashboard.Infrastructure/*.csproj ./Src/BBB-ApplicationDashboard.Infrastructure/

# Restore dependencies
RUN dotnet restore BBB-Application-Dashboard.sln

# Copy the source code
COPY Src/ ./Src/

# Build the application
WORKDIR /src/Src/BBB-ApplicationDashboard.Api
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "BBB-ApplicationDashboard.Api.dll"]
