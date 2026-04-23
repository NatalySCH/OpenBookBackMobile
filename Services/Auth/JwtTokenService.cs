using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenBooksBackMobile.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenBooksBackMobile.Services.Auth
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Usuario> _userManager;

        public JwtTokenService(IConfiguration config, UserManager<Usuario> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(Usuario usuario)
        {
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.UserName),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(usuario);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["JwtSettings:ExpiresInMinutes"])
                ),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}