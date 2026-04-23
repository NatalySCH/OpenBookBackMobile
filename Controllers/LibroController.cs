using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class LibroController : ControllerBase
{
    private readonly LibroService _libroService;
    private readonly CategoriaService _categoriaService;
    private readonly ApplicationDbContext _context;

    public LibroController(LibroService libroService, CategoriaService categoriaService,ApplicationDbContext context)
    {
        _libroService = libroService;
        _categoriaService = categoriaService;
        _context = context;
    }

    // 🔹 Obtener catálogo
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var libros = await _libroService.GetAllAsync();
        return Ok(libros);
    }

    // 🔹 Subir archivo
    [HttpPost("upload-libro")]
    public async Task<IActionResult> UploadLibro(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Archivo inválido");

        var url = await _libroService.UploadArchivoAsync(archivo);
        return Ok(new
        {
            url,
            fileName = Path.GetFileName(url)
        });
    }

    // 🔹 Subir portada
    [HttpPost("upload-portada")]
    public async Task<IActionResult> UploadPortada(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Archivo inválido");

        var url = await _libroService.UploadPortadaAsync(archivo);
        return Ok(new { url });
    }

    // 🔹 Crear libro
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(LibroCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var libro = await _libroService.CreateAsync(dto, userId);

        return Ok(new LibroResponseDto
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Autor = libro.Autor,
            Descripcion = libro.Descripcion,
            ArchivoUrl = libro.ArchivoUrl,
            PortadaUrl = libro.PortadaUrl
        });
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetLibros(
    string? query,
    int page = 1,
    int pageSize = 10,
    string? autor = null)
    {
        var (libros, total) = await _libroService.GetPagedAsync(query, page, pageSize, autor);

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            TotalPages = (int)Math.Ceiling((double)total / pageSize),
            Data = libros
        });
    }

    [Authorize]
    [HttpGet("{id}/descargar")]
    public async Task<IActionResult> DescargarLibro(int id)
    {
        var libro = await _context.Libros.FindAsync(id);

        if (libro == null)
            return NotFound();

        var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", libro.ArchivoUrl.TrimStart('/'));

        if (!System.IO.File.Exists(ruta))
            return NotFound("Archivo no encontrado");

        var bytes = await System.IO.File.ReadAllBytesAsync(ruta);

        return File(bytes, "application/epub+zip", $"{libro.Titulo}.epub");
    }

    [Authorize]
    [HttpPost("{id}/categorias")]
    public async Task<IActionResult> AsignarCategorias(int id, [FromBody] List<int> categoriasIds)
    {
        await _libroService.AssignCategoriasAsync(id, categoriasIds);
        return Ok();
    }

    [HttpGet("{id}/libros")]
    public async Task<IActionResult> GetLibrosByCategoria(int id)
    {
        var categoria = await _categoriaService.GetByIdWithBooksAsync(id);

        if (categoria == null)
            return NotFound();

        return Ok(categoria);
    }
}
