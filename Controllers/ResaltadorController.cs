using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.ResaltadorDtos;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResaltadoresController : ControllerBase
{
    private readonly ResaltadorService _service;

    public ResaltadoresController(ResaltadorService service)
    {
        _service = service;
    }

    // 🔹 Crear
    [HttpPost]
    public async Task<IActionResult> Create(ResaltadorCreateDto dto)
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
    public async Task<IActionResult> Update(int id, ResaltadorUpdateDto dto)
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