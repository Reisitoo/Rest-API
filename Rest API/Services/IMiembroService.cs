using Rest_API.DTOs;

namespace Rest_API.Services;

public interface IMiembroService
{
    // Nohely
    Task<MiembroResponse?> GetPerfilAsync(string userId);
    Task<MiembroResponse?> GetByIdAsync(string id);

    // Kener
    Task<List<MiembroResponse>> BuscarAsync(BuscarRequest request);
}
