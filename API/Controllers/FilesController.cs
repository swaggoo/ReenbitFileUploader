using API.DTOs;
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
    public async Task<ActionResult> Upload(IFormFile file)
    {
        await _fileService.UploadAsync(file);
        var sasToken = _fileService.GenerateSasToken(file.FileName);

        return Ok(sasToken);
    }
}
