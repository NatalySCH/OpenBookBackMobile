using System.ComponentModel.DataAnnotations;

namespace OpenBooksBackMobile.DTOs.CategoriaDtos
{
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string Nombre { get; set; }
    }
}
