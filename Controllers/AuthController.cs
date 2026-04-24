using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.Services.Auth;

namespace OpenBooksBackMobile.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly JwtTokenService _jwtService;

        public AuthController(UserManager<Usuario> userManager, JwtTokenService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var usuario = await _userManager.FindByEmailAsync(request.Correo);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            var validPassword = await _userManager.CheckPasswordAsync(usuario, request.Contrasena);

            if (!validPassword)
                return Unauthorized("Contraseña incorrecta");

            var token = await _jwtService.GenerateToken(usuario);

            return Ok(new
            {
                token,
                username = usuario.UserName,
                correo = usuario.Email
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var usuario = new Usuario
            {
                UserName = request.UserName,
                Email = request.Correo
            };

            var result = await _userManager.CreateAsync(usuario, request.Contrasena);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(usuario, "Usuario");

            return Ok(new
            {
                mensaje = "Usuario registrado correctamente"
            });
        }
    }
}
