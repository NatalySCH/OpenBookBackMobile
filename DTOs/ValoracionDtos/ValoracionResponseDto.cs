namespace OpenBooksBackMobile.DTOs.ValoracionDtos
{
    public class ValoracionResponseDto
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public string UsuarioId { get; set; }
        public int Puntuacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
