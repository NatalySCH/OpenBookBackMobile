namespace OpenBooksBackMobile.Entities
{
    public class Denuncia
    {
        public int Id { get; set; }

        public string IdDenunciante { get; set; }
        public Usuario UsuarioDenunciante { get; set; }

        public string IdDenunciado { get; set; }
        public Usuario UsuarioDenunciado { get; set; }

        public string Comentario { get; set; }
    }
}
