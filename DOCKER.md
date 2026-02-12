# Blood Connect Backend - Docker Deployment

This guide explains how to build and run the Blood Connect backend API using Docker.

## Prerequisites

- Docker Desktop installed
- Docker Compose (included with Docker Desktop)
- At least 4GB of RAM allocated to Docker

## Quick Start with Docker Compose

The easiest way to run the backend with SQL Server:

```bash
# Navigate to backend directory
cd backend

# Start both API and SQL Server
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Stop and remove volumes (clean slate)
docker-compose down -v
```

The API will be available at `http://localhost:5000`

## Building the Docker Image Only

To build just the backend API image:

```bash
cd backend
docker build -t bloodconnect-api:latest .
```

## Running the Container Manually

If you want to run the container without docker-compose:

```bash
docker run -d \
  --name bloodconnect-api \
  -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "ConnectionStrings__DefaultConnection=Server=your-sql-server;Database=BloodConnectDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;" \
  -e "Jwt__Secret=your-super-secret-jwt-key-change-this-in-production-min-32-chars" \
  bloodconnect-api:latest
```

## Configuration

### Environment Variables

The following environment variables can be configured:

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | Production |
| `ASPNETCORE_URLS` | URLs the app listens on | http://+:8080 |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | See docker-compose.yml |
| `Jwt__Secret` | JWT signing secret (min 32 chars) | Required |
| `Jwt__Issuer` | JWT issuer | BloodConnectAPI |
| `Jwt__Audience` | JWT audience | BloodConnectClient |
| `Jwt__ExpiryMinutes` | JWT token expiry time | 60 |
| `Cors__AllowedOrigins__0` | CORS allowed origin | http://localhost:5173 |

### Changing the Database Password

Edit `docker-compose.yml` and update both:
1. `sqlserver` service → `SA_PASSWORD`
2. `api` service → `ConnectionStrings__DefaultConnection` password

**Important**: Use a strong password in production!

### Adding Frontend CORS Origins

Add more allowed origins in `docker-compose.yml`:

```yaml
- Cors__AllowedOrigins__0=http://localhost:5173
- Cors__AllowedOrigins__1=http://localhost:3000
- Cors__AllowedOrigins__2=https://your-frontend-domain.com
```

## Ports

- **5000**: API HTTP endpoint (mapped from internal 8080)
- **1433**: SQL Server (exposed for external connections if needed)

## Health Check

The container includes a health check endpoint. Check container health:

```bash
docker ps
# or
docker inspect --format='{{.State.Health.Status}}' bloodconnect-api
```

## Database Migrations

The application automatically creates and seeds the database on first run. To manually run migrations:

```bash
# Exec into the running container
docker exec -it bloodconnect-api /bin/bash

# Run migrations (if you have EF tools in the image)
dotnet ef database update
```

## Logs

View application logs:

```bash
# Docker Compose
docker-compose logs -f api

# Individual container
docker logs -f bloodconnect-api
```

## Troubleshooting

### Database Connection Issues

If the API can't connect to SQL Server:

1. Check SQL Server is running: `docker ps | grep sqlserver`
2. Check SQL Server logs: `docker-compose logs sqlserver`
3. Verify connection string in `docker-compose.yml`
4. Ensure SQL Server health check passes

### Port Already in Use

If port 5000 is already in use, change it in `docker-compose.yml`:

```yaml
ports:
  - "5001:8080"  # Change 5000 to 5001
```

### Out of Memory

If containers fail to start, increase Docker memory:
- Docker Desktop → Settings → Resources → Memory → Increase to at least 4GB

### Container Won't Start

Check logs for errors:

```bash
docker-compose logs api
```

Common issues:
- Invalid connection string
- SQL Server not ready (wait for health check)
- Port conflicts
- Missing environment variables

## Production Deployment

For production deployment:

1. **Change secrets**: Update JWT secret and database password
2. **Use environment files**: Create `.env` file instead of hardcoding values
3. **Enable HTTPS**: Configure SSL certificates
4. **Use managed database**: Consider Azure SQL Database or AWS RDS instead of container
5. **Add monitoring**: Integrate with logging and monitoring solutions
6. **Set resource limits**: Add memory and CPU limits to containers

Example `.env` file:

```env
SA_PASSWORD=YourStrongProductionPassword!
JWT_SECRET=your-production-jwt-secret-min-32-characters-long
CORS_ORIGINS=https://your-production-frontend.com
```

Then reference in `docker-compose.yml`:

```yaml
environment:
  - Jwt__Secret=${JWT_SECRET}
```

## Multi-Stage Build

The Dockerfile uses a multi-stage build to:
- **Stage 1 (build)**: Uses SDK image to build the application
- **Stage 2 (publish)**: Publishes release build
- **Stage 3 (final)**: Uses smaller runtime-only image

This keeps the final image size small (~220MB vs ~800MB with SDK).

## Security

The container:
- ✅ Runs as non-root user (`appuser`)
- ✅ Uses official Microsoft base images
- ✅ Includes health checks
- ✅ Minimal attack surface (runtime image only)

## Further Reading

- [.NET Docker Documentation](https://learn.microsoft.com/en-us/dotnet/core/docker/introduction)
- [SQL Server on Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
