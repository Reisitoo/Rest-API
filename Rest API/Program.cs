using Rest_API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuración de Firebase ----------
var projectId = builder.Configuration["Firebase:ProjectId"]
    ?? throw new InvalidOperationException("Falta Firebase:ProjectId en la configuración.");
// El .gitignore solo excluye Config/firebase-credentials.json, así que el archivo DEBE estar en esa carpeta.
var credentialsPath = Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration["Firebase:CredentialsPath"] ?? "Config/firebase-credentials.json");

// Firestore (singleton: FirestoreDb es seguro para uso concurrente)
var firestoreDb = new FirestoreDbBuilder
{
    ProjectId = projectId,
    CredentialsPath = credentialsPath
}.Build();
builder.Services.AddSingleton(firestoreDb);

// ---------- Autenticación JWT (tokens de Firebase) ----------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// ---------- Servicios de la aplicación ----------
// AuthService usa HttpClient para llamar a la API REST de Firebase Auth.
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IMiembroService, MiembroService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // JSON en /openapi/v1.json
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
