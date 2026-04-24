namespace OpenBooksBackMobile.DTOs.ResenaDtos
{
    public class ResenaResponseDto
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public string UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Texto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
