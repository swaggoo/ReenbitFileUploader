using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IFileService
{
    Task UploadFileWithEmailMetadataAsync(IFormFile file, string email);
    string GenerateSASToken(string blobName);
}