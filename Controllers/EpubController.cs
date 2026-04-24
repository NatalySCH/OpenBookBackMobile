using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.Services.Interfaces;

[ApiController]
[Route("api/epub")]
public class EpubController : ControllerBase
{
    private readonly IEpubService _epubService;

    public EpubController(IEpubService epubService)
    {
        _epubService = epubService;
    }

    [HttpGet("{libroId}/manifest")]
    public async Task<IActionResult> GetManifest(int libroId)
    {
        try
        {
            var manifest = await _epubService.GetManifestAsync(libroId);
            return Ok(manifest);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{libroId}/resource")]
    public async Task<IActionResult> GetResource(int libroId, [FromQuery] string path)
    {
        try
        {
            var (data, contentType) = await _epubService.GetResourceAsync(libroId, path);

            return File(data, contentType);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}