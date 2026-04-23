using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.MarcadorDtos;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MarcadoresController : ControllerBase
{
    private readonly MarcadorService _service;

    public MarcadoresController(MarcadorService service)
    {
        _service = service;
    }

    // 🔹 Crear marcador
    [HttpPost]
    public async Task<IActionResult> Create(MarcadorCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _service.CreateAsync(dto, userId);

        return Ok(result);
    }

    // 🔹 Por usuario
    [HttpGet("usuario")]
    public async Task<IActionResult> GetByUsuario()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _service.GetByUsuarioAsync(userId);

        return Ok(result);
    }

    // 🔹 Por libro
    [HttpGet("libro/{libroId}")]
    public async Task<IActionResult> GetByLibro(int libroId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _service.GetByLibroAsync(libroId, userId);

        return Ok(result);
    }

    // 🔹 Update
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MarcadorUpdateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var ok = await _service.UpdateAsync(id, dto, userId);

        if (!ok) return NotFound();

        return NoContent();
    }

    // 🔹 Delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var ok = await _service.DeleteAsync(id, userId);

        if (!ok) return NotFound();

        return NoContent();
    }
}