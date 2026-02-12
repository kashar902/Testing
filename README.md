# Blood Connect Backend API

.NET 10 C# Web API for the Blood Connect donor management system.

## Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code (optional)

## Setup Instructions

### 1. Configure Database Connection

The application uses SQL Server LocalDB by default. The connection string is in `BloodConnect.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BloodConnectDB;Trusted_Connection=true;TrustServerCertificate=true"
}
```

### 2. Update Database

The database will be automatically created and seeded when the application starts. If you need to manually update:

```bash
cd backend
dotnet ef database update --project BloodConnect.Infrastructure --startup-project BloodConnect.API
```

### 3. Run the API

```bash
cd backend
dotnet run --project BloodConnect.API
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### 4. Access Swagger Documentation

Once the API is running, navigate to:
- `http://localhost:5000` or `https://localhost:5001`

## Project Structure

```
backend/
├── BloodConnect.API/              # Web API layer
│   ├── Controllers/              # API endpoints
│   ├── Middleware/               # Custom middleware
│   └── Program.cs                # Application entry point
├── BloodConnect.Core/            # Domain layer
│   ├── Entities/                 # Domain entities
│   ├── DTOs/                     # Data transfer objects
│   └── Interfaces/               # Repository interfaces
├── BloodConnect.Infrastructure/  # Data access layer
│   ├── Data/                     # DbContext & migrations
│   ├── Repositories/             # Repository implementations
│   └── Seeds/                    # Database seed data
└── BloodConnect.Services/        # Business logic layer
    └── Services/                 # Service implementations
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new staff user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh` - Refresh JWT token

### Donors
- `POST /api/donors` - Create new donor (no auth required)
- `GET /api/donors` - Get all donors (paginated, auth required)
- `GET /api/donors/{id}` - Get donor by ID (auth required)
- `GET /api/donors/coupon/{couponCode}` - Get donor by coupon (no auth required)
- `PUT /api/donors/{id}` - Update donor (auth required)
- `GET /api/donors/{id}/screenings` - Get donor screening history (auth required)

### Screenings
- `POST /api/screenings` - Create donation screening (no auth required)
- `GET /api/screenings` - Get all screenings (paginated, auth required)
- `GET /api/screenings/{id}` - Get screening by ID (auth required)
- `GET /api/screenings/donor/{donorId}` - Get screenings by donor (auth required)

### Branches
- `GET /api/branches` - Get all active branches (no auth required)
- `GET /api/branches/{id}` - Get branch by ID (auth required)
- `POST /api/branches` - Create branch (admin only)
- `PUT /api/branches/{id}` - Update branch (admin only)

### Deferral Reasons
- `GET /api/deferral-reasons` - Get all deferral reasons (no auth required)
- `GET /api/deferral-reasons/{id}` - Get deferral reason by ID (no auth required)

## Seeded Data

The database is automatically seeded with:
- 4 blood donation branches
- 7 deferral reasons
- 2 sample donors (Sarah Johnson, Michael Chen)

## Configuration

Key configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Jwt": {
    "Secret": "...",
    "Issuer": "BloodConnectAPI",
    "Audience": "BloodConnectClient",
    "ExpiryMinutes": "60"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

## Technologies Used

- .NET 10 Web API
- Entity Framework Core 10
- SQL Server
- JWT Bearer Authentication
- BCrypt.Net for password hashing
- Swagger/OpenAPI for API documentation

## Testing

To test the API:
1. Start the backend API
2. Use Swagger UI at `http://localhost:5000`
3. Or use Postman/curl to make requests
4. Create a staff user via `/api/auth/register` for authenticated endpoints

## Common Issues

### Database Connection Error
Ensure SQL Server LocalDB is installed and running.

### Port Already in Use
Change the port in `BloodConnect.API/Properties/launchSettings.json`

### CORS Error
Update `Cors:AllowedOrigins` in `appsettings.json` to include your frontend URL

