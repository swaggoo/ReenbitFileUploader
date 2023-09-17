using API.Controllers;
using API.DTOs;
using Azure;
using Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Web.Http;
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

        // Create a FileDto object
        var fileDto = new FileDto
        {
            Email = email,
            File = file.Object
        };

        var sasToken = "sas-token";

        _mockFileService.Setup(x => x.UploadFileWithEmailMetadataAsync(file.Object, email))
            .Returns(Task.CompletedTask);

        _mockFileService.Setup(x => x.GenerateSASToken(file.Object.FileName))
            .Returns(sasToken);

        // Act
        var result = await _filesController.Upload(fileDto); // Pass the FileDto object

        // Assert
        result.Should().BeOfType<ActionResult<FileToReturnDto>>();
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
        var fileDto = new FileDto // Create a FileDto object and set necessary properties
        {
            Email = "test@gmail.com",
            File = file.Object
        };
        var result = await _filesController.Upload(fileDto);

        // Assert
        result.Should().BeOfType<ActionResult<FileToReturnDto>>(); // Use the correct return type

        var objectResult = (ActionResult<FileToReturnDto>)result;
        objectResult.Result.Should().BeOfType<InternalServerErrorResult>(); // Check for internal server error
    }


}
