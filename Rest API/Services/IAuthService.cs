
using Google.Cloud.Firestore;
using Rest_API.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rest_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly FirestoreDb _firestoreDb;
        private readonly IConfiguration _configuration;

        public AuthService(
            HttpClient httpClient,
            FirestoreDb firestoreDb,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _firestoreDb = firestoreDb;
            _configuration = configuration;
        }

        // FERNANDO: REGISTRO
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            string apiKey = _configuration["Firebase:ApiKey"]
                ?? throw new Exception("No se encontró la API Key.");

            // Crear usuario en Firebase Authentication
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";

            var firebaseRequest = new
            {
                email = request.Email,
                password = request.Password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(
                url, firebaseRequest);

            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            var firebaseUser =
                JsonSerializer.Deserialize<FirebaseAuthResponse>(content);

            if (firebaseUser == null)
            {
                throw new Exception("No se recibió respuesta de Firebase.");
            }

            // Guardar perfil en Firestore
            var user = new Dictionary<string, object>
            {
                { "Id", firebaseUser.LocalId },
                { "Email", firebaseUser.Email },
                { "DisplayName", request.DisplayName },
                { "Username", request.Username },
                { "PhoneNumber", request.PhoneNumber },
                { "BirthDate", Timestamp.FromDateTime(
                    DateTime.SpecifyKind(
                        request.BirthDate,
                        DateTimeKind.Utc)) },
                { "Country", request.Country },
                { "Bio", request.Bio },
                { "Role", "Miembro" },
                { "CreatedAt", Timestamp.GetCurrentTimestamp() }
            };

            await _firestoreDb.Collection("users")
                .Document(firebaseUser.LocalId)
                .SetAsync(user);

            // Retornar respuesta
            return new AuthResponse
            {
                IdToken = firebaseUser.IdToken,
                LocalId = firebaseUser.LocalId,
                Email = firebaseUser.Email
            };
        }

        // Respuesta de Firebase
        private class FirebaseAuthResponse
        {
            [JsonPropertyName("idToken")]
            public string IdToken { get; set; } = "";

            [JsonPropertyName("localId")]
            public string LocalId { get; set; } = "";

            [JsonPropertyName("email")]
            public string Email { get; set; } = "";
        }
    }
}

