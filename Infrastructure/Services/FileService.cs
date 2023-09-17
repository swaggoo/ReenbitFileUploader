using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;
public class FileService : IFileService
{
    private readonly BlobServiceClient _blobServiceClient;

    public FileService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadFileWithEmailMetadataAsync(IFormFile file, string email)
    {
        BlobContainerClient containerClient = _blobServiceClient
            .GetBlobContainerClient(Constants.BlobContainerName);

        var blobClient = containerClient.GetBlobClient(file.FileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions()
            {
                Metadata = GetEmailAndSASTokenAsMetadata(email)
            });
        }
    }

    private string GenerateSASToken(string fileName)
    {
        var blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = Constants.BlobContainerName,
            BlobName = fileName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
        };

        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

        var storageSharedKeyCredential = new StorageSharedKeyCredential(
            Constants.StorageAccountName,
            Constants.StorageAccountKey);

        BlobSasQueryParameters sasQueryParameters = blobSasBuilder.ToSasQueryParameters(storageSharedKeyCredential);

        // Generate the SAS Token
        string sasToken = sasQueryParameters.ToString();

        return sasToken;
    }

    public string GenerateFileAccessUrl(string fileName)
    {
        var sasToken = GenerateSASToken(fileName);
        var accessUrl = $"https://{Constants.StorageAccountName}.blob.core.windows.net/{Constants.BlobContainerName}/{fileName}?{sasToken}";

        return accessUrl;
    }


    private Dictionary<string, string> GetEmailAndSASTokenAsMetadata(string email)
    {
        return new Dictionary<string, string>()
        {
            { "Email", email },
        };
    }
}
