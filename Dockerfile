# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY BloodConnect.slnx .

# Copy all project files
COPY BloodConnect.API/BloodConnectApi.csproj BloodConnect.API/
COPY BloodConnect.Core/BloodConnect.Core.csproj BloodConnect.Core/
COPY BloodConnect.Infrastructure/BloodConnect.Infrastructure.csproj BloodConnect.Infrastructure/
COPY BloodConnect.Services/BloodConnect.Services.csproj BloodConnect.Services/

# Restore dependencies
RUN dotnet restore BloodConnect.API/BloodConnectApi.csproj

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/BloodConnect.API
RUN dotnet build BloodConnectApi.csproj -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish BloodConnectApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks (optional)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user for security
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "BloodConnectApi.dll"]
