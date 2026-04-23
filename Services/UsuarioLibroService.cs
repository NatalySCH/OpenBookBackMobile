using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs.UsuarioLibroDtos;
using OpenBooksBackMobile.Entities;

public class UsuarioLibroService
{
    private readonly ApplicationDbContext _context;

    public UsuarioLibroService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddLibroAsync(string usuarioId, int libroId)
    {
        var existe = await _context.UsuarioLibros
            .AnyAsync(x => x.UsuarioId == usuarioId && x.LibroId == libroId);

        if (existe) return false;

        var nuevo = new UsuarioLibro
        {
            UsuarioId = usuarioId,
            LibroId = libroId
        };

        _context.UsuarioLibros.Add(nuevo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<UsuarioLibroDto>> GetBibliotecaAsync(string usuarioId)
    {
        return await _context.UsuarioLibros
            .Where(x => x.UsuarioId == usuarioId && x.EnBiblioteca)
            .Include(x => x.Libro)
            .OrderByDescending(x => x.UltimaLectura ?? x.FechaAgregado) // 🔥 clave
            .Select(x => new UsuarioLibroDto
            {
                LibroId = x.LibroId,
                Titulo = x.Libro.Titulo,
                Autor = x.Libro.Autor,
                PortadaUrl = x.Libro.PortadaUrl,
                Progreso = x.Progreso,
                PaginaActual = x.PaginaActual,
                EsFavorito = x.EsFavorito,
                UltimaLectura = x.UltimaLectura
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateProgresoAsync(string usuarioId, UpdateProgresoDto dto)
    {
        var registro = await _context.UsuarioLibros
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.LibroId == dto.LibroId);

        if (registro == null) return false;

        registro.Progreso = dto.Progreso;
        registro.PaginaActual = dto.PaginaActual;
        registro.UltimaLectura = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleFavoritoAsync(string usuarioId, int libroId)
    {
        var registro = await _context.UsuarioLibros
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.LibroId == libroId);

        if (registro == null) return false;

        registro.EsFavorito = !registro.EsFavorito;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveLibroAsync(string usuarioId, int libroId)
    {
        var registro = await _context.UsuarioLibros
            .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.LibroId == libroId);

        if (registro == null) return false;

        _context.UsuarioLibros.Remove(registro);
        await _context.SaveChangesAsync();

        return true;
    }
}
