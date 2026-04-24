using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.Services;
using System.Security.Claims;

namespace OpenBooksBackMobile.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(
            UsuarioService usuarioService,
            UserManager<Usuario> userManager)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
        }

        // 🔹 GET ALL (paginado)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsuarios(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _usuarioService.GetAllPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // 🔹 GET BY ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(string id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // 🔹 POST (crear usuario)
        [HttpPost]
        public async Task<IActionResult> PostUsuario([FromBody] UsuarioCreateDto dto)
        {
            var (usuario, errores) = await _usuarioService.CreateAsync(dto);

            if (usuario == null)
                return BadRequest(new { errores });

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpPost("upload-foto")]
        public async Task<IActionResult> UploadFotoPerfil(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo inválido");

            var url = await _usuarioService.UploadFotoPerfilAsync(archivo);
            return Ok(new { url });
        }

        // 🔹 PATCH (actualizar)
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUsuario(string id, [FromBody] UsuarioUpdateDto dto)
        {
            var result = await _usuarioService.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // 🔹 DELETE
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var result = await _usuarioService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("solicitar-recuperacion")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarRecuperacionDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Correo);

            if (usuario == null)
                return NotFound("Correo no registrado");

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

            // Simulación
            Console.WriteLine($"TOKEN: {token}");

            return Ok(new { token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);

            if (usuario == null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(usuario, dto.Token, dto.NuevaContraseña);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Contraseña actualizada");
        }
    }
}