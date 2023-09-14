using Azure.Storage;
using Azure.Storage.Blobs;
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

    public async Task UploadAsync(IFormFile file)
    {
        BlobContainerClient containerClient = _blobServiceClient
            .GetBlobContainerClient(Constants.BlobContainerName);

        var blobClient = containerClient.GetBlobClient(file.FileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }
    }

    public string GenerateSasToken(string blobName)
    {
        var blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = Constants.BlobContainerName,
            BlobName = blobName,
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
}
