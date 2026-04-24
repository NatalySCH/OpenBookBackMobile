namespace OpenBooksBackMobile.DTOs.DenunciaDtos
{
    public class DenunciaResponseDto
    {
        public int Id { get; set; }

        public string IdDenunciante { get; set; }
        public string NombreDenunciante { get; set; }

        public string IdDenunciado { get; set; }
        public string NombreDenunciado { get; set; }

        public string Comentario { get; set; }
    }
}
