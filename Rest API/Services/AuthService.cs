using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using Rest_API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Rest_API.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly FirestoreDb _db;
    private readonly string _apiKey;

    public AuthService(HttpClient http, FirestoreDb db, IConfiguration config)
    {
        _http = http;
        _db = db;
        _apiKey = config["Firebase:ApiKey"]
            ?? throw new InvalidOperationException("Falta Firebase:ApiKey en la configuración.");
    }

    // TODO (Fernando): signUp en Firebase + guardar perfil en Firestore con Role = Miembro
    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
        => throw new NotImplementedException();

    // TODO (Yahir): signInWithPassword en Firebase
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";

        var payload = new
        {
            email = request.Email,
            password = request.Password,
            returnSecureToken = true
        };

        using var response = await _http.PostAsJsonAsync(url, payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
            var code = error?.Error?.Message ?? "UNKNOWN";

            // Firebase a veces agrega texto después del código (ej. "TOO_MANY_ATTEMPTS_TRY_LATER : ...")
            if (code.StartsWith("INVALID_LOGIN_CREDENTIALS")
                || code.StartsWith("EMAIL_NOT_FOUND")
                || code.StartsWith("INVALID_PASSWORD"))
                throw new UnauthorizedAccessException("Correo o contraseña incorrectos.");

            if (code.StartsWith("USER_DISABLED"))
                throw new UnauthorizedAccessException("La cuenta está deshabilitada.");

            if (code.StartsWith("TOO_MANY_ATTEMPTS_TRY_LATER"))
                throw new UnauthorizedAccessException("Demasiados intentos. Intenta más tarde.");

            throw new InvalidOperationException($"Error de Firebase al iniciar sesión: {code}");
        }

        var data = await response.Content.ReadFromJsonAsync<FirebaseSignInResponse>()
            ?? throw new InvalidOperationException("Respuesta vacía de Firebase.");

        return new AuthResponse
        {
            UserId = data.LocalId,
            Email = data.Email,
            IdToken = data.IdToken,
            RefreshToken = data.RefreshToken,
            ExpiresIn = int.TryParse(data.ExpiresIn, out var seconds) ? seconds : 3600
        };
    }

    // Modelos privados para deserializar las respuestas de Firebase
    private record FirebaseSignInResponse(
        string LocalId, string Email, string IdToken, string RefreshToken, string ExpiresIn);

    private record FirebaseErrorResponse(FirebaseErrorDetail? Error);
    private record FirebaseErrorDetail(string? Message);
}