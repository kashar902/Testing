using BloodConnect.Core.DTOs;

namespace BloodConnect.Services.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    string GenerateJwtToken(UserDto user);
    string GenerateRefreshToken();
}

