using API.Controllers;
using API.DTOs;
using Azure;
using Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Web.Http;
using Xunit;

namespace Tests.Controllers;
public class FilesControllerTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly FilesController _filesController;

    public FilesControllerTests()
    {
        #region Dependencies
        _fileServiceMock = new Mock<IFileService>();
        #endregion

        #region SUT
        _filesController = new FilesController(_fileServiceMock.Object);
        #endregion
    }

    [Fact]
    public async Task Upload_ReturnsOk_WithFileToReturnDto_WithAccessUrl()
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("example.docx");

        // Arrange
        var fileDto = new FileDto
        {
            File = mockFile.Object,
            Email = "test@example.com"
        };

        var expectedAccessUrl = "https://example.com/file.txt";
        _fileServiceMock.Setup(m => m.GenerateFileAccessUrl(fileDto.File.FileName)).Returns(expectedAccessUrl);

        // Act
        var actionResult = await _filesController.Upload(fileDto);

        // Assert
        var result = actionResult.Result as OkObjectResult;
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<FileToReturnDto>();

        var fileToReturnDto = result.Value as FileToReturnDto;
        fileToReturnDto.Should().NotBeNull();
        fileToReturnDto!.Url.Should().Be(expectedAccessUrl);
    }

    [Fact]
    public async Task Upload_ReturnsBadRequest_IfFileIsNotValid()
    {
        // Arrange
        var fileDto = new FileDto
        {
            File = null
        };

        // Act
        var actionResult = await _filesController.Upload(fileDto);

        // Assert
        var result = actionResult.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(400);
        result.Value.Should().Be("File is not valid");
    }








}
