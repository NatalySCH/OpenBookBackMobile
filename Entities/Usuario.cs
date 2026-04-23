using Microsoft.AspNetCore.Identity;

namespace OpenBooksBackMobile.Entities
{
    public class Usuario : IdentityUser<int>
    {
        public bool Estado { get; set; } = true;
        public bool Sancionado { get; set; } = false;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        public ICollection<UsuarioLibro> Libros { get; set; } = new List<UsuarioLibro>();
    }
}
