namespace OpenBooksBackMobile.DTOs
{
    public class UsuarioCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public string Contraseña { get; set; }
        public string? FotoPerfilUrl { get; set; }
    }
}
