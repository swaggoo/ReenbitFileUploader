using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Core;
using FluentAssertions;
using Azure.Storage.Blobs.Models;

namespace Tests.Services;
public class FileServiceTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly IFileService _fileService;

    public FileServiceTests()
    {
        #region Dependencies
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        #endregion

        #region SUT
        _fileService = new FileService(_blobServiceClientMock.Object);
        #endregion
    }

    [Fact]
    public async Task UploadFileWithEmailMetadataAsync_Success()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var email = "test@example.com";
        var containerClientMock = new Mock<BlobContainerClient>();

        _blobServiceClientMock
            .Setup(client => client.GetBlobContainerClient(Constants.BlobContainerName))
            .Returns(containerClientMock.Object);

        var blobClientMock = new Mock<BlobClient>();
        containerClientMock
            .Setup(container => container.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        var stream = new MemoryStream();

        // Act
        await _fileService.UploadFileWithEmailMetadataAsync(file.Object, email);

        // Assert
        blobClientMock.Verify(
            client => client.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), default),
            Times.Once);
    }

    [Fact]
    public void GenerateSASToken_Should_ReturnSASQueryParameters_When_BlobNameIsValid()
    {
        // Arrange
        var blobName = "my-blob.txt";

        var storageSharedKeyCredential = new StorageSharedKeyCredential(Constants.StorageAccountName, Constants.StorageAccountKey);
        var blobSasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = Constants.BlobContainerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
        };
        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Act
        var sasToken = _fileService.GenerateSASToken(blobName);

        // Assert
        sasToken.Should().Be(blobSasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString());
    }
}
