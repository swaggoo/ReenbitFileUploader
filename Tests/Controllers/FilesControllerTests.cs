using API.Controllers;
using Azure;
using Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Controllers;
public class FilesControllerTests
{
    private readonly Mock<IFileService> _mockFileService;
    private readonly FilesController _filesController;

    public FilesControllerTests()
    {
        #region Dependencies
        _mockFileService = new Mock<IFileService>();
        #endregion

        #region SUT
        _filesController = new FilesController(_mockFileService.Object);
        #endregion
    }

    [Fact]
    public async Task Upload_Should_ReturnOkResult_When_FileIsUploadedSuccessfully()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var email = "test@gmail.com";
        var sasToken = "sas-token";

        _mockFileService.Setup(x => x.UploadFileWithEmailMetadataAsync(file.Object, email))
            .Returns(Task.CompletedTask);

        _mockFileService.Setup(x => x.GenerateSASToken(file.Object.FileName))
            .Returns(sasToken);

        // Act
        var result = await _filesController.Upload(file.Object);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Upload_Should_ReturnInternalServerError_When_FileUploadFails()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var exceptionMessage = "File upload failed.";

        _mockFileService.Setup(x => x.UploadFileWithEmailMetadataAsync(file.Object, It.IsAny<string>()))
            .ThrowsAsync(new RequestFailedException(exceptionMessage));

        // Act
        var result = await _filesController.Upload(file.Object);

        // Assert
        result.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }
}
