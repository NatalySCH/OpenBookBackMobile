namespace OpenBooksBackMobile.Mappings
{
    using AutoMapper;
    using OpenBooksBackMobile.Entities;
    using OpenBooksBackMobile.DTOs;

    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioResponseDto>();
        }
    }
}
