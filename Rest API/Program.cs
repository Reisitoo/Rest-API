using Rest_API.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var projectId = builder.Configuration["Firebase:ProjectId"]
    ?? throw new InvalidOperationException("Falta Firebase:ProjectId en la configuración.");
var credentialsPath = Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration["Firebase:CredentialsPath"] ?? "Config/firebase-credentials.json");

var firestoreDb = new FirestoreDbBuilder
{
    ProjectId = projectId,
    CredentialsPath = credentialsPath
}.Build();
builder.Services.AddSingleton(firestoreDb);

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

builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IMiembroService, MiembroService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
