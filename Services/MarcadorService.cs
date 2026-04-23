using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.MarcadorDtos;

public class MarcadorService
{
    private readonly ApplicationDbContext _context;

    public MarcadorService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 Crear marcador
    public async Task<MarcadorResponseDto> CreateAsync(MarcadorCreateDto dto, string usuarioId)
    {
        var marcador = new Marcador
        {
            LibroId = dto.LibroId,
            UsuarioId = usuarioId,
            Pagina = dto.Pagina,
            Fecha = DateTime.UtcNow
        };

        _context.Marcadores.Add(marcador);
        await _context.SaveChangesAsync();

        var libro = await _context.Libros.FindAsync(dto.LibroId);

        return new MarcadorResponseDto
        {
            Id = marcador.Id,
            LibroId = marcador.LibroId,
            TituloLibro = libro?.Titulo ?? "",
            Pagina = marcador.Pagina,
            Fecha = marcador.Fecha
        };
    }

    // 🔹 Obtener por usuario
    public async Task<List<MarcadorResponseDto>> GetByUsuarioAsync(string usuarioId)
    {
        return await _context.Marcadores
            .Where(m => m.UsuarioId == usuarioId)
            .Include(m => m.Libro)
            .Select(m => new MarcadorResponseDto
            {
                Id = m.Id,
                LibroId = m.LibroId,
                TituloLibro = m.Libro.Titulo,
                Pagina = m.Pagina,
                Fecha = m.Fecha
            })
            .ToListAsync();
    }

    // 🔹 Obtener por libro
    public async Task<List<MarcadorResponseDto>> GetByLibroAsync(int libroId, string usuarioId)
    {
        return await _context.Marcadores
            .Where(m => m.LibroId == libroId && m.UsuarioId == usuarioId)
            .Select(m => new MarcadorResponseDto
            {
                Id = m.Id,
                LibroId = m.LibroId,
                TituloLibro = m.Libro.Titulo,
                Pagina = m.Pagina,
                Fecha = m.Fecha
            })
            .ToListAsync();
    }

    // 🔹 Update marcador
    public async Task<bool> UpdateAsync(int id, MarcadorUpdateDto dto, string usuarioId)
    {
        var marcador = await _context.Marcadores
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

        if (marcador == null)
            return false;

        marcador.Pagina = dto.Pagina;

        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 Delete marcador
    public async Task<bool> DeleteAsync(int id, string usuarioId)
    {
        var marcador = await _context.Marcadores
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

        if (marcador == null)
            return false;

        _context.Marcadores.Remove(marcador);
        await _context.SaveChangesAsync();
        return true;
    }
}
