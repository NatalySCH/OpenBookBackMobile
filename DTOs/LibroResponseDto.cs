namespace OpenBooksBackMobile.DTOs
{
    public class LibroResponseDto
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string? Descripcion { get; set; }

        public string? PortadaUrl { get; set; }
        public string ArchivoUrl { get; set; }

        public bool EsPublico { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string? UsuarioCreadorId { get; set; }
    }
}
