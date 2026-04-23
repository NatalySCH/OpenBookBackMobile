using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.DTOs.ValoracionDtos;
using OpenBooksBackMobile.Entities;
using System.ComponentModel.DataAnnotations;

public class ValoracionService
{
    private readonly ApplicationDbContext _context;

    public ValoracionService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔹 CREATE
    public async Task<ValoracionResponseDto> CreateAsync(string usuarioId, ValoracionCreateDto dto)
    {
        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            throw new ValidationException("La puntuación debe estar entre 1 y 5.");

        var libroExists = await _context.Libros.AnyAsync(l => l.Id == dto.LibroId);
        if (!libroExists)
            throw new KeyNotFoundException("El libro no existe.");

        var exists = await _context.Valoraciones
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId && v.LibroId == dto.LibroId);

        if (exists != null)
            throw new ValidationException("Ya has valorado este libro.");

        var valoracion = new Valoracion
        {
            UsuarioId = usuarioId,
            LibroId = dto.LibroId,
            Puntuacion = dto.Puntuacion,
            Fecha = DateTime.UtcNow
        };

        _context.Valoraciones.Add(valoracion);
        await _context.SaveChangesAsync();

        return new ValoracionResponseDto
        {
            Id = valoracion.Id,
            LibroId = valoracion.LibroId,
            UsuarioId = valoracion.UsuarioId,
            Puntuacion = valoracion.Puntuacion,
            Fecha = valoracion.Fecha
        };
    }

    // 🔹 UPDATE
    public async Task<bool> UpdateAsync(string usuarioId, int libroId, ValoracionUpdateDto dto)
    {
        if (dto.Puntuacion < 1 || dto.Puntuacion > 5)
            return false;

        var valoracion = await _context.Valoraciones
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId && v.LibroId == libroId);

        if (valoracion == null)
            return false;

        valoracion.Puntuacion = dto.Puntuacion;
        valoracion.Fecha = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 DELETE
    public async Task<bool> DeleteAsync(string usuarioId, int libroId)
    {
        var valoracion = await _context.Valoraciones
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId && v.LibroId == libroId);

        if (valoracion == null)
            return false;

        _context.Valoraciones.Remove(valoracion);
        await _context.SaveChangesAsync();
        return true;
    }

    // 🔹 GET BY LIBRO
    public async Task<List<ValoracionResponseDto>> GetAllByLibroAsync(int libroId)
    {
        return await _context.Valoraciones
            .Where(v => v.LibroId == libroId)
            .OrderByDescending(v => v.Fecha)
            .Select(v => new ValoracionResponseDto
            {
                Id = v.Id,
                LibroId = v.LibroId,
                UsuarioId = v.UsuarioId,
                Puntuacion = v.Puntuacion,
                Fecha = v.Fecha
            })
            .ToListAsync();
    }

    // 🔹 TOP 5
    public async Task<List<ValoracionResponseDto>> GetTop5Async()
    {
        var topLibros = await _context.Valoraciones
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

        foreach (var top in topLibros)
        {
            var valoracion = await _context.Valoraciones
                .Where(v => v.LibroId == top.LibroId)
                .OrderByDescending(v => v.Fecha)
                .FirstOrDefaultAsync();

            if (valoracion != null)
            {
                result.Add(new ValoracionResponseDto
                {
                    Id = valoracion.Id,
                    LibroId = valoracion.LibroId,
                    UsuarioId = valoracion.UsuarioId,
                    Puntuacion = valoracion.Puntuacion,
                    Fecha = valoracion.Fecha
                });
            }
        }

        return result;
    }
}