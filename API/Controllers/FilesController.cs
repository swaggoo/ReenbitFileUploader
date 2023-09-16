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
    public async Task<ActionResult> Upload(IFormFile file)
    {
        var email = "nazarnyrka00@gmail.com";
        if (file == null)
        {
            return BadRequest("Upload a correct file");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("The email address is in the incorrect format");
        }

        try
        {
            await _fileService.UploadFileWithEmailMetadataAsync(file, email);
            var sasToken = _fileService.GenerateSASToken(file.FileName);

            return Ok(sasToken);
        }
        catch (RequestFailedException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
