using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IFileService
{
    Task UploadAsync(IFormFile file);
    string GenerateSasToken(string fileName);
}