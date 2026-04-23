namespace OpenBooksBackMobile.Entities
{
    public class Libro
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public string? PortadaUrl { get; set; }
        public string ArchivoUrl { get; set; } = string.Empty;

        public bool EsPublico { get; set; } = true;
        public int? UsuarioCreadorId { get; set; }
        public Usuario? UsuarioCreador { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<UsuarioLibro> Usuarios { get; set; } = new List<UsuarioLibro>();
    }
}
