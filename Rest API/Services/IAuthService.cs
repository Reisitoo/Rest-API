using Rest_API.DTOs;

namespace Rest_API.Services;

public interface IAuthService
{
    // Fernando
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    // Yahir
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
