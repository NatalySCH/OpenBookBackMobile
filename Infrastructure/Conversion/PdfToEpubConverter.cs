using OpenBooksBackMobile.Services.Interfaces;
using System.IO.Compression;
using System.Net;
using System.Text;
using UglyToad.PdfPig;

public class PdfToEpubConverter : IPdfToEpubConverter
{
    public async Task<string> ConvertAsync(string pdfPath, string outputDirectory, string title)
    {
        var sb = new StringBuilder();
        using (var document = PdfDocument.Open(pdfPath))
        {
            foreach (var page in document.GetPages())
            {
                sb.AppendLine($"<h2>Página {page.Number}</h2>");
                sb.AppendLine($"<p>{WebUtility.HtmlEncode(page.Text)}</p>");
                sb.AppendLine("<hr/>");
            }
        }

        var bookId = Guid.NewGuid().ToString();
        var fileName = $"{bookId}.epub";
        var fullPath = Path.Combine(outputDirectory, fileName);

        using (var zip = ZipFile.Open(fullPath, ZipArchiveMode.Create))
        {
            // 1. mimetype — SIN compresión (spec EPUB)
            var mime = zip.CreateEntry("mimetype", CompressionLevel.NoCompression);
            using (var w = new StreamWriter(mime.Open(), new UTF8Encoding(false)))
                await w.WriteAsync("application/epub+zip");

            // 2. META-INF/container.xml — apunta al .opf
            var container = zip.CreateEntry("META-INF/container.xml");
            using (var w = new StreamWriter(container.Open(), new UTF8Encoding(false)))
                await w.WriteAsync(BuildContainerXml());

            // 3. OEBPS/content.opf — manifiesto del libro
            var opf = zip.CreateEntry("OEBPS/content.opf");
            using (var w = new StreamWriter(opf.Open(), new UTF8Encoding(false)))
                await w.WriteAsync(BuildOpf(bookId, title));

            // 4. OEBPS/toc.ncx — tabla de contenido (compatibilidad EPUB 2)
            var ncx = zip.CreateEntry("OEBPS/toc.ncx");
            using (var w = new StreamWriter(ncx.Open(), new UTF8Encoding(false)))
                await w.WriteAsync(BuildNcx(bookId, title));

            // 5. OEBPS/content.xhtml — el contenido real
            var content = zip.CreateEntry("OEBPS/content.xhtml");
            using (var w = new StreamWriter(content.Open(), new UTF8Encoding(false)))
                await w.WriteAsync(BuildXhtml(sb.ToString(), title));
        }

        return fileName;
    }

    // --- Helpers ---

    private string BuildContainerXml() => """
        <?xml version="1.0" encoding="UTF-8"?>
        <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
          <rootfiles>
            <rootfile full-path="OEBPS/content.opf"
                      media-type="application/oebps-package+xml"/>
          </rootfiles>
        </container>
        """;

    private string BuildOpf(string bookId, string title) => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <package version="2.0"
                 xmlns="http://www.idpf.org/2007/opf"
                 unique-identifier="bookid">
          <metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
            <dc:title>{WebUtility.HtmlEncode(title)}</dc:title>
            <dc:identifier id="bookid">urn:uuid:{bookId}</dc:identifier>
            <dc:language>es</dc:language>
          </metadata>
          <manifest>
            <item id="content" href="content.xhtml"
                  media-type="application/xhtml+xml"/>
            <item id="ncx"    href="toc.ncx"
                  media-type="application/x-dtbncx+xml"/>
          </manifest>
          <spine toc="ncx">
            <itemref idref="content"/>
          </spine>
        </package>
        """;

    private string BuildNcx(string bookId, string title) => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <ncx xmlns="http://www.daisy.org/z3986/2005/ncx/" version="2005-1">
          <head>
            <meta name="dtb:uid" content="urn:uuid:{bookId}"/>
          </head>
          <docTitle><text>{WebUtility.HtmlEncode(title)}</text></docTitle>
          <navMap>
            <navPoint id="np1" playOrder="1">
              <navLabel><text>Inicio</text></navLabel>
              <content src="content.xhtml"/>
            </navPoint>
          </navMap>
        </ncx>
        """;

    private string BuildXhtml(string body, string title) => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE html>
        <html xmlns="http://www.w3.org/1999/xhtml">
          <head>
            <meta charset="utf-8"/>
            <title>{WebUtility.HtmlEncode(title)}</title>
          </head>
          <body>
            {body}
          </body>
        </html>
        """;
}