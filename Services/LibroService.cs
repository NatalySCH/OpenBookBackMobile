using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.Entities;
using OpenBooksBackMobile.DTOs;
using OpenBooksBackMobile.Services.Interfaces;

public class LibroService
{
    private readonly ApplicationDbContext _context;
    private readonly IPdfToEpubConverter _converter;
    private readonly IWebHostEnvironment _env;

    public LibroService(ApplicationDbContext context, IPdfToEpubConverter converter, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
        _converter = converter;
    }

    public async Task<List<LibroResponseDto>> GetAllAsync()
    {
        var libros = await _context.Libros
            .Where(l => l.EsPublico)
            .Include(l => l.LibroCategorias)
                .ThenInclude(lc => lc.Categoria)
            .Include(l => l.Valoraciones)
            .Include(l => l.Resenas)
            .ToListAsync();

        return libros.Select(MapLibro).ToList();
    }

    // 🔹 Subir archivo (libro)
    public async Task<string> UploadArchivoAsync(IFormFile archivo)
    {
        var carpeta = Path.Combine(_env.WebRootPath, "uploads/libros");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var nombre = $"{Guid.NewGuid()}_{archivo.FileName}";
        var ruta = Path.Combine(carpeta, nombre);

        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/libros/{nombre}";
    }

    // 🔹 Subir portada
    public async Task<string> UploadPortadaAsync(IFormFile archivo)
    {
        var carpeta = Path.Combine(_env.WebRootPath, "uploads/portadas");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var nombre = $"{Guid.NewGuid()}_{archivo.FileName}";
        var ruta = Path.Combine(carpeta, nombre);

        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        return $"/uploads/portadas/{nombre}";
    }

    // 🔹 Crear libro
    public async Task<LibroResponseDto> CreateAsync(LibroCreateDto dto, string usuarioId)
    {
        if (string.IsNullOrEmpty(dto.ArchivoUrl))
            throw new Exception("Archivo requerido");

        var libro = new Libro
        {
            Titulo = dto.Titulo,
            Autor = dto.Autor,
            Descripcion = dto.Descripcion,
            EsPublico = dto.EsPublico,
            PortadaUrl = dto.PortadaUrl,
            UsuarioCreadorId = usuarioId,
            FechaCreacion = DateTime.UtcNow,
            LibroCategorias = new List<LibroCategoria>(),
            Valoraciones = new List<Valoracion>(),
            Resenas = new List<Resena>()
        };
        var extension = Path.GetExtension(dto.ArchivoUrl).ToLower();

        var physicalPath = Path.Combine(
        _env.WebRootPath,
        dto.ArchivoUrl.TrimStart('/')
    );

        if (extension == ".epub")
        {
            libro.ArchivoUrl = dto.ArchivoUrl;
        }

        else if (extension == ".pdf")
        {
            libro.SourceFileUrl = dto.ArchivoUrl;

            var epubFile = await _converter.ConvertAsync(
                physicalPath,
                Path.Combine(_env.WebRootPath, "uploads/libros"),
                libro.Titulo
            );

            libro.ArchivoUrl = $"/uploads/libros/{epubFile}";
        }
        else
        {
            throw new Exception("Formato no soportado");
        }

        if (dto.CategoriaIds != null && dto.CategoriaIds.Any())
        {
            libro.LibroCategorias = dto.CategoriaIds
                .Select(id => new LibroCategoria
                {
                    CategoriaId = id
                })
                .ToList();
        }

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        var libroDb = await _context.Libros
            .Where(l => l.Id == libro.Id)
            .Include(l => l.LibroCategorias)
                .ThenInclude(lc => lc.Categoria)
            .Include(l => l.Valoraciones)
            .Include(l => l.Resenas)
            .FirstAsync();

        return MapLibro(libroDb);
    }

    public async Task<(IEnumerable<LibroResponseDto>, int)> GetPagedAsync(
 string? query,
 int page,
 int pageSize,
 string? autor,
 int? categoriaId)
    {
        var dbQuery = _context.Libros
            .Where(l => l.EsPublico)
            .Include(l => l.LibroCategorias)
                .ThenInclude(lc => lc.Categoria)
            .Include(l => l.Valoraciones)
            .Include(l => l.Resenas)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            dbQuery = dbQuery.Where(l =>
                l.Titulo.Contains(query) ||
                (l.Descripcion != null && l.Descripcion.Contains(query)));
        }

        if (!string.IsNullOrEmpty(autor))
        {
            dbQuery = dbQuery.Where(l => l.Autor.Contains(autor));
        }

        if (categoriaId.HasValue)
        {
            dbQuery = dbQuery.Where(l =>
                l.LibroCategorias.Any(lc => lc.CategoriaId == categoriaId.Value));
        }

        int total = await dbQuery.CountAsync();

        var librosDb = await dbQuery
            .OrderBy(l => l.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var libros = librosDb.Select(MapLibro);

        return (libros, total);
    }


    public async Task AssignCategoriasAsync(int libroId, List<int> categoriaIds)
    {
        var libro = await _context.Libros
            .Include(l => l.LibroCategorias)
            .FirstOrDefaultAsync(l => l.Id == libroId);

        if (libro == null)
            throw new Exception("Libro no encontrado");

        _context.LibroCategorias.RemoveRange(libro.LibroCategorias);

        libro.LibroCategorias = categoriaIds
            .Select(id => new LibroCategoria
            {
                LibroId = libroId,
                CategoriaId = id
            })
            .ToList();

        await _context.SaveChangesAsync();
    }

    private LibroResponseDto MapLibro(Libro l)
    {
        return new LibroResponseDto
        {
            Id = l.Id,
            Titulo = l.Titulo,
            Autor = l.Autor,
            Descripcion = l.Descripcion,
            ArchivoUrl = l.ArchivoUrl,
            PortadaUrl = l.PortadaUrl,
            EsPublico = l.EsPublico,
            FechaCreacion = l.FechaCreacion,
            UsuarioCreadorId = l.UsuarioCreadorId,

            Categorias = l.LibroCategorias?
                .Select(lc => lc.Categoria.Nombre)
                .ToList() ?? new List<string>(),

            TotalValoraciones = l.Valoraciones?.Count ?? 0,

            PromedioValoracion = (l.Valoraciones != null && l.Valoraciones.Any())
                ? l.Valoraciones.Average(v => v.Puntuacion)
                : 0,

            
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.LibroCategorias)
            .Include(l => l.Valoraciones)
            .Include(l => l.Resenas)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (libro == null)
            return false;

        // 🔹 Limpiar relaciones (buena práctica)
        _context.LibroCategorias.RemoveRange(libro.LibroCategorias);

        _context.Valoraciones.RemoveRange(libro.Valoraciones);
        _context.Resenas.RemoveRange(libro.Resenas);

        _context.Libros.Remove(libro);

        await _context.SaveChangesAsync();
        return true;
    }
}
