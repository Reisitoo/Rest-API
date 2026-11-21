using Rest_API.DTOs;
using Google.Cloud.Firestore;

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
    public Task<AuthResponse> LoginAsync(LoginRequest request)
        => throw new NotImplementedException();
}
