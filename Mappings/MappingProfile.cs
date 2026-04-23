using AutoMapper;
using OpenBooksBackMobile.DTOs.CategoriaDtos;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.Entities;

namespace OpenBooksBackMobile.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioResponseDto>();
            CreateMap<Categoria, CategoriaResponseDto>();
            CreateMap<CategoriaCreateDto, Categoria>();
            CreateMap<CategoriaUpdateDto, Categoria>();


            CreateMap<Libro, LibroResponseDto>();
        }
    }
}
