using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs.DenunciaDtos;
using OpenBooksBackMobile.Services;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class DenunciaController : ControllerBase
{
    private readonly DenunciaService _denunciaService;

    public DenunciaController(DenunciaService denunciaService)
    {
        _denunciaService = denunciaService;
    }

    // 🔹 Crear denuncia (usuario autenticado)
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CrearDenuncia([FromBody] DenunciaCreateDto dto)
    {
        var idDenunciante = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(idDenunciante))
            return Unauthorized("No se pudo identificar el usuario.");

        if (idDenunciante == dto.IdDenunciado)
            return BadRequest("No puedes denunciarte a ti mismo.");

        var resultado = await _denunciaService.CrearDenunciaAsync(idDenunciante, dto);

        if (resultado == null)
            return BadRequest("Denunciante o denunciado no existe.");

        return CreatedAtAction(nameof(GetDenuncias), new { }, resultado);
    }

    // 🔹 Listar denuncias (solo admin)
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> GetDenuncias(int pagina = 1, int tamanoPagina = 10)
    {
        var (denuncias, total) = await _denunciaService.ListarDenunciasPaginadoAsync(pagina, tamanoPagina);

        var resultado = new
        {
            PaginaActual = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total,
            TotalPaginas = (int)Math.Ceiling((double)total / tamanoPagina),
            Datos = denuncias
        };

        return Ok(resultado);
    }

    // 🔹 Eliminar denuncia (solo admin)
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarDenuncia(int id)
    {
        var eliminado = await _denunciaService.EliminarDenunciaAsync(id);

        if (!eliminado)
            return NotFound("Denuncia no encontrada.");

        return NoContent();
    }
}