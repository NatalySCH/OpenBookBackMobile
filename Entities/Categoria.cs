namespace OpenBooksBackMobile.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<Libro> Libros { get; set; } = new();
        public ICollection<LibroCategoria> LibroCategorias { get; set; } = new List<LibroCategoria>();
    }
}
