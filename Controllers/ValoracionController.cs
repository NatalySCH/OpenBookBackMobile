using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs.ValoracionDtos;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ValoracionesController : ControllerBase
{
    private readonly ValoracionService _service;

    public ValoracionesController(ValoracionService service)
    {
        _service = service;
    }

    // 🔹 CREATE
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ValoracionCreateDto dto)
    {
        var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized("Token inválido.");

        var result = await _service.CreateAsync(usuarioId, dto);
        return Ok(result);
    }

    // 🔹 UPDATE
    [HttpPut("{libroId}")]
    [Authorize]
    public async Task<IActionResult> Update(int libroId, [FromBody] ValoracionUpdateDto dto)
    {
        var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized("Token inválido.");

        var updated = await _service.UpdateAsync(usuarioId, libroId, dto);

        if (!updated)
            return BadRequest("No se pudo actualizar la valoración.");

        return Ok("Actualizado correctamente.");
    }

    // 🔹 DELETE
    [HttpDelete("{libroId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int libroId)
    {
        var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized("Token inválido.");

        var deleted = await _service.DeleteAsync(usuarioId, libroId);

        if (!deleted)
            return NotFound("Valoración no encontrada.");

        return Ok("Eliminada correctamente.");
    }

    // 🔹 GET por libro
    [HttpGet("libro/{libroId}")]
    public async Task<IActionResult> GetByLibro(int libroId)
    {
        var result = await _service.GetAllByLibroAsync(libroId);
        return Ok(result);
    }

    // 🔹 TOP 5
    [HttpGet("top5")]
    public async Task<IActionResult> Top5()
    {
        var result = await _service.GetTop5Async();
        return Ok(result);
    }
}