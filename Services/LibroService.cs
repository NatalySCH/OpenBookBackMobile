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

    // 🔹 Obtener catálogo
    public async Task<List<LibroResponseDto>> GetAllAsync()
    {
        return await _context.Libros
            .Where(l => l.EsPublico)
            .Include(l => l.LibroCategorias)
                .ThenInclude(lc => lc.Categoria)
            .Include(l => l.Valoraciones)
            .Select(l => MapLibro(l))
            .ToListAsync();
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
            FechaCreacion = DateTime.UtcNow
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
            foreach (var catId in dto.CategoriaIds)
            {
                libro.LibroCategorias.Add(new LibroCategoria
                {
                    CategoriaId = catId
                });
            }
        }

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        return MapLibro(libro);
    }

    public async Task<(IEnumerable<LibroResponseDto>, int)> GetPagedAsync(
    string? query,
    int page,
    int pageSize,
    string? autor)
    {
        var dbQuery = _context.Libros
            .Where(l => l.EsPublico)
            .Include(l => l.LibroCategorias)
                .ThenInclude(lc => lc.Categoria)
            .Include(l => l.Valoraciones)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            dbQuery = dbQuery.Where(l =>
                l.Titulo.Contains(query) ||
                l.Descripcion.Contains(query));
        }

        if (!string.IsNullOrEmpty(autor))
        {
            dbQuery = dbQuery.Where(l => l.Autor.Contains(autor));
        }

        int total = await dbQuery.CountAsync();

        var libros = await dbQuery
            .OrderBy(l => l.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => MapLibro(l))
            .ToListAsync();

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

            Categorias = l.LibroCategorias
                .Select(lc => lc.Categoria.Nombre)
                .ToList(),

            TotalValoraciones = l.Valoraciones.Count,
            PromedioValoracion = l.Valoraciones.Any()
                ? l.Valoraciones.Average(v => v.Puntuacion)
                : 0
        };
    }
}
