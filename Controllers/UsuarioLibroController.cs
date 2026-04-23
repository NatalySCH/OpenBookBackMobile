using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs.UsuarioLibroDtos;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioLibroController : ControllerBase
{
    private readonly UsuarioLibroService _service;

    public UsuarioLibroController(UsuarioLibroService service)
    {
        _service = service;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // 📚 Obtener biblioteca del usuario
    [HttpGet("biblioteca")]
    public async Task<IActionResult> GetBiblioteca()
    {
        var userId = GetUserId();

        var libros = await _service.GetBibliotecaAsync(userId);

        return Ok(libros);
    }

    // ➕ Agregar libro a biblioteca
    [HttpPost("{libroId}")]
    public async Task<IActionResult> AddLibro(int libroId)
    {
        var userId = GetUserId();

        var ok = await _service.AddLibroAsync(userId, libroId);

        if (!ok)
            return BadRequest("El libro ya está en la biblioteca");

        return Ok("Libro agregado");
    }

    // ❌ Eliminar libro de biblioteca
    [HttpDelete("{libroId}")]
    public async Task<IActionResult> RemoveLibro(int libroId)
    {
        var userId = GetUserId();

        var ok = await _service.RemoveLibroAsync(userId, libroId);

        if (!ok)
            return NotFound();

        return NoContent();
    }

    // 📖 Actualizar progreso
    [HttpPatch("progreso")]
    public async Task<IActionResult> UpdateProgreso(UpdateProgresoDto dto)
    {
        var userId = GetUserId();

        var ok = await _service.UpdateProgresoAsync(userId, dto);

        if (!ok)
            return NotFound();

        return NoContent();
    }

    // ⭐ Toggle favorito
    [HttpPatch("favorito/{libroId}")]
    public async Task<IActionResult> ToggleFavorito(int libroId)
    {
        var userId = GetUserId();

        var ok = await _service.ToggleFavoritoAsync(userId, libroId);

        if (!ok)
            return NotFound();

        return NoContent();
    }
}
