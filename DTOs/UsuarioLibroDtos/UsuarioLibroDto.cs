namespace OpenBooksBackMobile.DTOs.UsuarioLibroDtos
{
    public class UsuarioLibroDto
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string? PortadaUrl { get; set; }

        public double Progreso { get; set; }
        public int PaginaActual { get; set; }
        public bool EsFavorito { get; set; }

        public DateTime? UltimaLectura { get; set; }
    }
}
