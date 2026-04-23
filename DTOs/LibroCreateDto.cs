namespace OpenBooksBackMobile.DTOs
{
    public class LibroCreateDto
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string? Descripcion { get; set; }

        public string ArchivoUrl { get; set; }
        public string? PortadaUrl { get; set; }

        public List<int>? CategoriaIds { get; set; }

        public bool EsPublico { get; set; } = false;
    }
}
