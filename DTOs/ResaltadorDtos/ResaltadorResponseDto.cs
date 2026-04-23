namespace OpenBooksBackMobile.DTOs.ResaltadorDtos
{
    public class ResaltadorResponseDto
    {
        public int Id { get; set; }

        public int LibroId { get; set; }
        public string TituloLibro { get; set; }

        public string Href { get; set; }

        public string CfiRange { get; set; }

        public string Color { get; set; }
    }
}
