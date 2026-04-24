using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.ResenaDtos;
using OpenBooksBackMobile.Entities;

public class ResenaService
{
    private readonly ApplicationDbContext _context;

    public ResenaService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 Crear reseña
    public async Task<ResenaResponseDto> CreateAsync(string usuarioId, ResenaCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Texto))
            throw new Exception("El texto es obligatorio");

        var libro = await _context.Libros.FindAsync(dto.LibroId);
        if (libro == null)
            throw new KeyNotFoundException("Libro no encontrado");

        var usuario = await _context.Users.FindAsync(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        var resena = new Resena
        {
            UsuarioId = usuarioId,
            LibroId = dto.LibroId,
            Texto = dto.Texto.Trim(),
            Fecha = DateTime.UtcNow
        };

        _context.Resenas.Add(resena);
        await _context.SaveChangesAsync();

        return new ResenaResponseDto
        {
            Id = resena.Id,
            LibroId = resena.LibroId,
            UsuarioId = usuarioId,
            NombreUsuario = usuario.UserName,
            Texto = resena.Texto,
            Fecha = resena.Fecha
        };
    }

    // 🔹 Update
    public async Task<bool> UpdateAsync(int id, string usuarioId, ResenaUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Texto))
            return false;

        var resena = await _context.Resenas
            .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

        if (resena == null)
            return false;

        resena.Texto = dto.Texto.Trim();
        resena.Fecha = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 Delete
    public async Task<bool> DeleteAsync(int id, string usuarioId)
    {
        var resena = await _context.Resenas
            .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuarioId);

        if (resena == null)
            return false;

        _context.Resenas.Remove(resena);
        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 Get por libro
    public async Task<List<ResenaResponseDto>> GetByLibroAsync(int libroId)
    {
        return await _context.Resenas
            .Where(r => r.LibroId == libroId)
            .OrderByDescending(r => r.Fecha)
            .Select(r => new ResenaResponseDto
            {
                Id = r.Id,
                LibroId = r.LibroId,
                UsuarioId = r.UsuarioId,
                NombreUsuario = r.Usuario.UserName,
                Texto = r.Texto,
                Fecha = r.Fecha
            })
            .ToListAsync();
    }
}