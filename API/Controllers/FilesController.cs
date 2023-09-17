using API.DTOs;
using Azure;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class FilesController : BaseController
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    public async Task<ActionResult<FileToReturnDto>> Upload([FromForm] FileDto fileDto)
    {
        if (!IsFileValid(fileDto.File))
        {
            return BadRequest("File is not valid");
        }

        await _fileService.UploadFileWithEmailMetadataAsync(fileDto.File, fileDto.Email);
        var accessUrl = _fileService.GenerateFileAccessUrl(fileDto.File.FileName);

        var fileToReturnDto = new FileToReturnDto
        {
            Url = accessUrl
        };

        return Ok(fileToReturnDto);
    }

    private bool IsFileValid(IFormFile file) => file != null && Path.GetExtension(file.FileName) == ".docx";
}
