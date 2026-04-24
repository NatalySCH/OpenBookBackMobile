using OpenBooksBackMobile.Data;
using OpenBooksBackMobile.DTOs.EpubDtos;
using VersOne.Epub;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using OpenBooksBackMobile.Services.Interfaces;

public class EpubService : IEpubService
{
    private readonly ApplicationDbContext _context;

    public EpubService(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetRutaLibro(string archivoUrl)
    {
        return Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            archivoUrl.TrimStart('/')
        );
    }

    public async Task<BookManifestDto> GetManifestAsync(int libroId)
    {
        Console.WriteLine("ENTRÓ A GET MANIFEST");

        var libro = await _context.Libros
            .FirstOrDefaultAsync(l => l.Id == libroId);

        if (libro == null)
            throw new Exception("Libro no encontrado");

        Console.WriteLine($"LIBRO ID: {libroId}");
        Console.WriteLine($"ARCHIVO URL: {libro.ArchivoUrl}");

        if (string.IsNullOrEmpty(libro.ArchivoUrl))
            throw new Exception("El archivo EPUB no tiene ruta");

        var ruta = GetRutaLibro(libro.ArchivoUrl);

        Console.WriteLine($"RUTA CALCULADA: {ruta}");
        Console.WriteLine($"EXISTE ARCHIVO: {File.Exists(ruta)}");

        if (!File.Exists(ruta))
            throw new Exception("Archivo EPUB no encontrado en el servidor");

        try
        {
            using var epubStream = File.OpenRead(ruta);
            var epub = await EpubReader.ReadBookAsync(epubStream);

            Console.WriteLine("EPUB cargado correctamente");
            Console.WriteLine($"Titulo: {epub.Title}");

            return new BookManifestDto
            {
                Id = libro.Id,
                Titulo = epub.Title,
                Autor = epub.Author,
                ReadingOrder = BuildReadingOrder(epub),
                Resources = BuildResources(epub),
                Toc = BuildToc(epub)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR AL LEER EPUB:");
            Console.WriteLine(ex.Message);

            throw;
        }
    }

    public async Task<(byte[] Data, string ContentType)> GetResourceAsync(int libroId, string path)
    {
        var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == libroId);

        if (libro == null)
            throw new Exception("Libro no encontrado");

        if (string.IsNullOrEmpty(libro.ArchivoUrl))
            throw new Exception("Archivo no disponible");

        var ruta = GetRutaLibro(libro.ArchivoUrl);

        if (!File.Exists(ruta))
            throw new Exception("Archivo EPUB no encontrado");

        // Leer EPUB con VersOne
        using var epubStream = File.OpenRead(ruta);
        var epub = await EpubReader.ReadBookAsync(epubStream);

        var normalizedPath = NormalizeHref(path);
        normalizedPath = Uri.UnescapeDataString(normalizedPath);

        var localFile = epub.Content.AllFiles.GetLocalFileByFilePath(normalizedPath);

        if (localFile == null)
            throw new Exception($"Recurso no encontrado: {normalizedPath}");

        // Leer directamente del ZIP
        using var zip = ZipFile.OpenRead(ruta);

        var entryPath = localFile.FilePath.Replace("\\", "/");

        var entry = zip.Entries.FirstOrDefault(e =>
            string.Equals(e.FullName.Replace("\\", "/"), entryPath, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
            throw new Exception($"Entrada ZIP no encontrada: {entryPath}");

        using var entryStream = entry.Open();
        using var ms = new MemoryStream();
        await entryStream.CopyToAsync(ms);

        var contentType = GetMime(localFile.ContentType);

        return (ms.ToArray(), contentType);
    }

    private string NormalizeHref(string path)
    {
        return path.Replace("\\", "/").Trim().TrimStart('/');
    }

    private string GetMime(EpubContentType contentType)
    {
        return contentType switch
        {
            EpubContentType.XHTML_1_1 => "application/xhtml+xml",
            EpubContentType.CSS => "text/css",
            EpubContentType.IMAGE_JPEG => "image/jpeg",
            EpubContentType.IMAGE_PNG => "image/png",
            EpubContentType.IMAGE_GIF => "image/gif",
            EpubContentType.IMAGE_SVG => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }

    private List<ReadingOrderItem> BuildReadingOrder(EpubBook epub)
    {
        return epub.ReadingOrder.Select(item => new ReadingOrderItem
        {
            Href = NormalizeHref(item.FilePath),
            MediaType = GetMime(item.ContentType) // 👈 agrega esto
        }).ToList();
    }

    private List<ResourceItem> BuildResources(EpubBook epub)
    {
        return epub.Content.AllFiles.Local.Select(file => new ResourceItem
        {
            Href = NormalizeHref(file.FilePath),
            MediaType = GetMime(file.ContentType)
        }).ToList();
    }

    private List<TocLinkDto> BuildToc(EpubBook epub)
    {
        var toc = new List<TocLinkDto>();

        void AddLinks(IEnumerable<EpubNavigationItem> items)
        {
            foreach (var item in items)
            {
                if (item.Link != null && !string.IsNullOrEmpty(item.Link.ContentFilePath))
                {
                    toc.Add(new TocLinkDto
                    {
                        Titulo = item.Title ?? string.Empty,
                        Href = NormalizeHref(item.Link.ContentFilePath)
                    });
                }

                if (item.NestedItems != null && item.NestedItems.Count > 0)
                {
                    AddLinks(item.NestedItems);
                }
            }
        }

        if (epub.Navigation != null && epub.Navigation.Count > 0)
        {
            AddLinks(epub.Navigation);
        }

        return toc;
    }
}
