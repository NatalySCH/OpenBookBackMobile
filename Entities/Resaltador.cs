namespace OpenBooksBackMobile.Entities
{
    public class Resaltador
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int LibroId { get; set; }
        public Libro Libro { get; set; }

        public string Href { get; set; }
        public string CfiRange { get; set; }
        public string Color { get; set; }
    }
}
