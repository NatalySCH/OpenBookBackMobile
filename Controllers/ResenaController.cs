using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs.ResenaDtos;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ResenaController : ControllerBase
{
    private readonly ResenaService _service;

    public ResenaController(ResenaService service)
    {
        _service = service;
    }

    private string UsuarioId =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    // 🔹 Crear
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ResenaCreateDto dto)
    {
        var result = await _service.CreateAsync(UsuarioId, dto);
        return Ok(result);
    }

    // 🔹 Update
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] ResenaUpdateDto dto)
    {
        var ok = await _service.UpdateAsync(id, UsuarioId, dto);

        if (!ok)
            return BadRequest("No se pudo actualizar la reseña");

        return Ok("Reseña actualizada");
    }

    // 🔹 Delete
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id, UsuarioId);

        if (!ok)
            return NotFound("Reseña no encontrada");

        return Ok("Reseña eliminada");
    }

    // 🔹 Get por libro
    [HttpGet("libro/{libroId}")]
    public async Task<IActionResult> GetByLibro(int libroId)
    {
        var resenas = await _service.GetByLibroAsync(libroId);
        return Ok(resenas);
    }
}