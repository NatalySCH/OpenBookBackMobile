namespace OpenBooksBackMobile.DTOs.ResaltadorDtos
{
    public class ResaltadorCreateDto
    {
        public int LibroId { get; set; }

        public string Href { get; set; }

        public string CfiRange { get; set; }

        public string Color { get; set; } = "#FFFF00";
    }
}
