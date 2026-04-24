using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs.SugerenciaDtos;
using OpenBooksBackMobile.Entities;

namespace OpenBooksBackMobile.Services
{
    public class SugerenciaService
    {
        private readonly ApplicationDbContext _context;

        public SugerenciaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SugerenciaResponseDto?> CrearSugerenciaAsync(string usuarioId, SugerenciaCreateDto dto)
        {
            var usuario = await _context.Users.FindAsync(usuarioId);

            if (usuario == null)
                return null;

            var sugerencia = new Sugerencia
            {
                UsuarioId = usuarioId,
                Comentario = dto.Comentario,
                Usuario = usuario
            };

            _context.Sugerencias.Add(sugerencia);
            await _context.SaveChangesAsync();

            return new SugerenciaResponseDto
            {
                Id = sugerencia.Id,
                UsuarioId = usuario.Id,
                NombreUsuario = usuario.UserName,
                Comentario = sugerencia.Comentario
            };
        }

        public async Task<(IEnumerable<SugerenciaResponseDto> Sugerencias, int TotalCount)> ListarSugerenciasPaginadoAsync(int pagina, int tamanoPagina)
        {
            var query = _context.Sugerencias
                .Include(s => s.Usuario)
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            var sugerencias = await query
                .OrderByDescending(s => s.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(s => new SugerenciaResponseDto
                {
                    Id = s.Id,
                    UsuarioId = s.UsuarioId,
                    NombreUsuario = s.Usuario.UserName,
                    Comentario = s.Comentario
                })
                .ToListAsync();

            return (sugerencias, totalCount);
        }

        public async Task<bool> EliminarSugerenciaAsync(int id)
        {
            var sugerencia = await _context.Sugerencias.FindAsync(id);
            if (sugerencia == null) return false;

            _context.Sugerencias.Remove(sugerencia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
