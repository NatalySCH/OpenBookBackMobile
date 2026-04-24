using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs.DenunciaDtos;
using OpenBooksBackMobile.Entities;

namespace OpenBooksBackMobile.Services
{
    public class DenunciaService
    {
        private readonly ApplicationDbContext _context;

        public DenunciaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DenunciaResponseDto?> CrearDenunciaAsync(string idDenunciante, DenunciaCreateDto dto)
        {
            var denunciante = await _context.Users.FindAsync(idDenunciante);
            var denunciado = await _context.Users.FindAsync(dto.IdDenunciado);

            if (denunciante == null || denunciado == null)
                return null;

            var denuncia = new Denuncia
            {
                IdDenunciante = idDenunciante,
                IdDenunciado = dto.IdDenunciado,
                Comentario = dto.Comentario,
                UsuarioDenunciante = denunciante,
                UsuarioDenunciado = denunciado
            };

            _context.Denuncias.Add(denuncia);
            await _context.SaveChangesAsync();

            return new DenunciaResponseDto
            {
                Id = denuncia.Id,
                IdDenunciante = idDenunciante,
                NombreDenunciante = denunciante.UserName,
                IdDenunciado = dto.IdDenunciado,
                NombreDenunciado = denunciado.UserName,
                Comentario = denuncia.Comentario
            };
        }

        public async Task<(IEnumerable<DenunciaResponseDto> Denuncias, int TotalCount)> ListarDenunciasPaginadoAsync(int pagina, int tamanoPagina)
        {
            var query = _context.Denuncias
                .Include(d => d.UsuarioDenunciante)
                .Include(d => d.UsuarioDenunciado)
                .AsQueryable();

            int totalCount = await query.CountAsync();

            var denuncias = await query
                .OrderByDescending(d => d.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(d => new DenunciaResponseDto
                {
                    Id = d.Id,
                    IdDenunciante = d.IdDenunciante,
                    NombreDenunciante = d.UsuarioDenunciante.UserName,
                    IdDenunciado = d.IdDenunciado,
                    NombreDenunciado = d.UsuarioDenunciado.UserName,
                    Comentario = d.Comentario
                })
                .ToListAsync();

            return (denuncias, totalCount);
        }

        public async Task<bool> EliminarDenunciaAsync(int id)
        {
            var denuncia = await _context.Denuncias.FindAsync(id);
            if (denuncia == null) return false;

            _context.Denuncias.Remove(denuncia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
