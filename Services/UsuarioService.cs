using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.Shared;
using Microsoft.AspNetCore.Mvc;


namespace OpenBooksBackMobile.Services
{
    public class UsuarioService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public UsuarioService(UserManager<Usuario> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<UsuarioResponseDto>> GetAllAsync()
        {
            var usuarios = await _userManager.Users.ToListAsync();

            var lista = new List<UsuarioResponseDto>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var dto = _mapper.Map<UsuarioResponseDto>(user);
                dto.NombreRol = roles.FirstOrDefault();

                lista.Add(dto);
            }

            return lista;
        }

        public async Task<UsuarioResponseDto?> GetByIdAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return null;

            var roles = await _userManager.GetRolesAsync(usuario);

            var dto = _mapper.Map<UsuarioResponseDto>(usuario);
            dto.NombreRol = roles.FirstOrDefault();

            return dto;
        }

        public async Task<bool> UpdateAsync(string id, UsuarioUpdateDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                usuario.UserName = dto.UserName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                usuario.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.NombreCompleto))
                usuario.NombreCompleto = dto.NombreCompleto;

            var result = await _userManager.UpdateAsync(usuario);

            if (!result.Succeeded) return false;

            // Cambio de contraseña
            if (!string.IsNullOrWhiteSpace(dto.Contraseña))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                await _userManager.ResetPasswordAsync(usuario, token, dto.Contraseña);
            }

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return false;

            var result = await _userManager.DeleteAsync(usuario);
            return result.Succeeded;
        }

        public async Task<(UsuarioResponseDto? usuario, IEnumerable<string>? errores)> CreateAsync(UsuarioCreateDto dto)
        {
            var usuario = new Usuario
            {
                UserName = dto.UserName,
                Email = dto.Email,
                NombreCompleto = dto.NombreCompleto
            };

            var result = await _userManager.CreateAsync(usuario, dto.Contraseña);

            if (!result.Succeeded)
                return (null, result.Errors.Select(e => e.Description));

            var usuarioDto = _mapper.Map<UsuarioResponseDto>(usuario);

            return (usuarioDto, null);
        }

        public async Task<PagedResult<UsuarioResponseDto>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var query = _userManager.Users;

            var total = await query.CountAsync();

            var usuarios = await query
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var lista = new List<UsuarioResponseDto>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var dto = _mapper.Map<UsuarioResponseDto>(user);
                dto.NombreRol = roles.FirstOrDefault();

                lista.Add(dto);
            }

            return new PagedResult<UsuarioResponseDto>
            {
                Results = lista,
                TotalRecords = total,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };
        }

    }
}
