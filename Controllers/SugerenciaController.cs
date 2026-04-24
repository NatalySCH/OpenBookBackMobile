using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OpenBooksBackMobile.DTOs.SugerenciaDtos;
using OpenBooksBackMobile.Services;
using System.Security.Claims;

namespace OpeenBookBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SugerenciaController : ControllerBase
    {
        private readonly SugerenciaService _sugerenciaService;

        public SugerenciaController(SugerenciaService sugerenciaService)
        {
            _sugerenciaService = sugerenciaService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearSugerencia([FromBody] SugerenciaCreateDto dto)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var resultado = await _sugerenciaService.CrearSugerenciaAsync(usuarioId, dto);

            if (resultado == null)
                return BadRequest("Usuario no existe.");

            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetSugerencias(int pagina = 1, int tamanoPagina = 10)
        {
            var (sugerencias, total) =
                await _sugerenciaService.ListarSugerenciasPaginadoAsync(pagina, tamanoPagina);

            return Ok(new
            {
                PaginaActual = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = total,
                TotalPaginas = (int)Math.Ceiling((double)total / tamanoPagina),
                Datos = sugerencias
            });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSugerencia(int id)
        {
            bool eliminado = await _sugerenciaService.EliminarSugerenciaAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}
