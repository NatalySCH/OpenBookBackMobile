using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.ResaltadorDtos;

public class ResaltadorService
{
    private readonly ApplicationDbContext _context;

    public ResaltadorService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 Crear resaltado
    public async Task<ResaltadorResponseDto> CreateAsync(ResaltadorCreateDto dto, string usuarioId)
    {
        var resaltado = new Resaltador
        {
            LibroId = dto.LibroId,
            UsuarioId = usuarioId,
            Href = dto.Href,
            CfiRange = dto.CfiRange,
            Color = dto.Color
        };

        _context.Resaltadores.Add(resaltado);
        await _context.SaveChangesAsync();

        var libro = await _context.Libros.FindAsync(dto.LibroId);

        return new ResaltadorResponseDto
        {
            Id = resaltado.Id,
            LibroId = resaltado.LibroId,
            TituloLibro = libro?.Titulo ?? "",
            Href = resaltado.Href,
            CfiRange = resaltado.CfiRange,
            Color = resaltado.Color
        };
    }

    // 🔹 Por usuario
    public async Task<List<ResaltadorResponseDto>> GetByUsuarioAsync(string usuarioId)
    {
        return await _context.Resaltadores
            .Where(r => r.UsuarioId == usuarioId)
            .Include(r => r.Libro)
            .Select(r => new ResaltadorResponseDto
            {
                Id = r.Id,
                LibroId = r.LibroId,
                TituloLibro = r.Libro.Titulo,
                Href = r.Href,
                CfiRange = r.CfiRange,
                Color = r.Color
            })
            .ToListAsync();
    }

    // 🔹 Por libro
    public async Task<List<ResaltadorResponseDto>> GetByLibroAsync(int libroId, string usuarioId)
    {
        return await _context.Resaltadores
            .Where(r => r.LibroId == libroId && r.UsuarioId == usuarioId)
            .Select(r => new ResaltadorResponseDto
            {
                Id = r.Id,
                LibroId = r.LibroId,
                TituloLibro = r.Libro.Titulo,
                Href = r.Href,
                CfiRange = r.CfiRange,
                Color = r.Color
            })
            .ToListAsync();
    }

    // 🔹 Update
    public async Task<bool> UpdateAsync(int id, ResaltadorUpdateDto dto, string usuarioId)
    {
        var resaltado = await _context.Resaltadores
            .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

        if (resaltado == null)
            return false;

        resaltado.CfiRange = dto.CfiRange;
        resaltado.Color = dto.Color;

        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 Delete
    public async Task<bool> DeleteAsync(int id, string usuarioId)
    {
        var resaltado = await _context.Resaltadores
            .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

        if (resaltado == null)
            return false;

        _context.Resaltadores.Remove(resaltado);
        await _context.SaveChangesAsync();
        return true;
    }
}