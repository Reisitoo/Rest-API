using Rest_API.DTOs;
using Google.Cloud.Firestore;

namespace Rest_API.Services;

public class MiembroService : IMiembroService
{
    private readonly FirestoreDb _db;

    public MiembroService(FirestoreDb db)
    {
        _db = db;
    }

    // TODO (Nohely)
    public Task<MiembroResponse?> GetPerfilAsync(string userId)
        => throw new NotImplementedException();

    // TODO (Nohely)
    public Task<MiembroResponse?> GetByIdAsync(string id)
        => throw new NotImplementedException();

    // TODO (Kener)
    public Task<List<MiembroResponse>> BuscarAsync(BuscarRequest request)
        => throw new NotImplementedException();
}
