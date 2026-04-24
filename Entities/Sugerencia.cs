namespace OpenBooksBackMobile.Entities
{
    public class Sugerencia
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string Comentario { get; set; }
    }
}
