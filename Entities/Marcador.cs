namespace OpenBooksBackMobile.Entities
{
    public class Marcador
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int LibroId { get; set; }
        public Libro Libro { get; set; }

        public int Pagina { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
