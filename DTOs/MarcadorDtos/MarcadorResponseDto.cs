namespace OpenBooksBackMobile.DTOs.MarcadorDtos
{
    public class MarcadorResponseDto
    {
        public int Id { get; set; }

        public int LibroId { get; set; }
        public string TituloLibro { get; set; }

        public int Pagina { get; set; }

        public DateTime Fecha { get; set; }
    }
}
