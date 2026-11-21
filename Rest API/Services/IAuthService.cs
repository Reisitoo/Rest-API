using Microsoft.IdentityModel.Tokens;
using Rest_API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Rest_API.Services;

public interface IAuthService
{
    // Fernando
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    // Yahir
    Task<AuthResponse> LoginAsync(LoginRequest request);
    
}
