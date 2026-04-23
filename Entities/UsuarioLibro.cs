namespace OpenBooksBackMobile.Entities
{
    public class UsuarioLibro
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int LibroId { get; set; }
        public Libro Libro { get; set; }

        public double Progreso { get; set; } = 0.0;
        public int PaginaActual { get; set; }

        public DateTime? UltimaLectura { get; set; }

        public bool EnBiblioteca { get; set; } = true;
        public bool EsFavorito { get; set; } = false;

        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
    }
}
