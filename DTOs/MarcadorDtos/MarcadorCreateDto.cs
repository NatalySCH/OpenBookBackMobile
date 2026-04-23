
namespace OpenBooksBackMobile.DTOs.MarcadorDtos
{
    using System.ComponentModel.DataAnnotations;

    public class MarcadorCreateDto
    {
        [Required]
        public int LibroId { get; set; }

        [Required]
        public int Pagina { get; set; }
    }
}
