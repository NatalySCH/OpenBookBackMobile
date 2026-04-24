using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.DTOs;
using System.ComponentModel.DataAnnotations;
using OpenBooksBackMobile.DTOs.ValoracionDtos;

public class ValoracionService
{
    private readonly ApplicationDbContext _context;

    public ValoracionService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 CREAR VALORACIÓN
    public async Task<ValoracionResponseDto> CreateAsync(string idUsuario, ValoracionCreateDto dto)
    {
        if (dto == null)
            throw new ValidationException("Los datos no pueden ser nulos.");

        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            throw new ValidationException("La puntuación debe estar entre 1 y 5.");

        var usuario = await _context.Users.FindAsync(idUsuario);
        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        var libro = await _context.Libros.FindAsync(dto.LibroId);
        if (libro == null)
            throw new KeyNotFoundException("Libro no encontrado.");

        var existe = await _context.Valoraciones
            .FirstOrDefaultAsync(v => v.UsuarioId == idUsuario && v.LibroId == dto.LibroId);

        if (existe != null)
            throw new ValidationException("Ya has valorado este libro.");

        var valoracion = new Valoracion
        {
            UsuarioId = idUsuario,
            LibroId = dto.LibroId,
            Puntuacion = dto.Puntuacion,
            Fecha = DateTime.UtcNow
        };

        _context.Valoraciones.Add(valoracion);
        await _context.SaveChangesAsync();

        return new ValoracionResponseDto
        {
            Id = valoracion.Id,
            UsuarioId = idUsuario,
            LibroId = dto.LibroId,
            Puntuacion = dto.Puntuacion,
            Fecha = valoracion.Fecha
        };
    }

    // 🔹 ACTUALIZAR VALORACIÓN
    public async Task<bool> UpdateAsync(string idUsuario, int libroId, ValoracionUpdateDto dto)
    {
        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            return false;

        var valoracion = await _context.Valoraciones
            .FirstOrDefaultAsync(v => v.UsuarioId == idUsuario && v.LibroId == libroId);

        if (valoracion == null)
            return false;

        valoracion.Puntuacion = dto.Puntuacion;
        valoracion.Fecha = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 ELIMINAR VALORACIÓN
    public async Task<bool> DeleteAsync(string idUsuario, int libroId)
    {
        var valoracion = await _context.Valoraciones
            .FirstOrDefaultAsync(v => v.UsuarioId == idUsuario && v.LibroId == libroId);

        if (valoracion == null)
            return false;

        _context.Valoraciones.Remove(valoracion);
        await _context.SaveChangesAsync();

        return true;
    }

    // 🔹 GET VALORACIONES POR LIBRO
    public async Task<List<ValoracionResponseDto>> GetAllByLibroAsync(int libroId)
    {
        return await _context.Valoraciones
            .Where(v => v.LibroId == libroId)
            .OrderByDescending(v => v.Fecha)
            .Select(v => new ValoracionResponseDto
            {
                Id = v.Id,
                UsuarioId = v.UsuarioId,
                LibroId = v.LibroId,
                Puntuacion = v.Puntuacion,
                Fecha = v.Fecha
            })
            .ToListAsync();
    }

    // 🔹 TOP 5 LIBROS (por promedio)
    public async Task<List<ValoracionResponseDto>> GetTop5Async()
    {
        var top = await _context.Valoraciones
            .GroupBy(v => v.LibroId)
            .Select(g => new
            {
                LibroId = g.Key,
                Promedio = g.Average(x => x.Puntuacion)
            })
            .OrderByDescending(x => x.Promedio)
            .Take(5)
            .ToListAsync();

        var result = new List<ValoracionResponseDto>();

        foreach (var item in top)
        {
            var valoracion = await _context.Valoraciones
                .Where(v => v.LibroId == item.LibroId)
                .OrderByDescending(v => v.Puntuacion)
                .FirstOrDefaultAsync();

            if (valoracion != null)
            {
                result.Add(new ValoracionResponseDto
                {
                    Id = valoracion.Id,
                    UsuarioId = valoracion.UsuarioId,
                    LibroId = valoracion.LibroId,
                    Puntuacion = valoracion.Puntuacion,
                    Fecha = valoracion.Fecha
                });
            }
        }

        return result;
    }
}