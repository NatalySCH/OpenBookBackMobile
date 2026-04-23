using UglyToad.PdfPig;
using System.IO.Compression;
using System.Text;
using OpenBooksBackMobile.Services.Interfaces;

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
                sb.AppendLine($"<p>{page.Text}</p>");
                sb.AppendLine("<hr/>");
            }
        }

        var fileName = $"{Guid.NewGuid()}.epub";
        var fullPath = Path.Combine(outputDirectory, fileName);

        using (var zip = ZipFile.Open(fullPath, ZipArchiveMode.Create))
        {
            // 1. mimetype (OBLIGATORIO EPUB)
            var mime = zip.CreateEntry("mimetype", CompressionLevel.NoCompression);
            using (var stream = new StreamWriter(mime.Open()))
            {
                await stream.WriteAsync("application/epub+zip");
            }

            // 2. content
            var content = zip.CreateEntry("OEBPS/content.xhtml");
            using (var stream = new StreamWriter(content.Open()))
            {
                await stream.WriteAsync(WrapHtml(sb.ToString(), title));
            }

            // 3. container.xml
            var container = zip.CreateEntry("META-INF/container.xml");
            using (var stream = new StreamWriter(container.Open()))
            {
                await stream.WriteAsync(@"
                <?xml version='1.0' encoding='UTF-8'?>
                <container version='1.0' xmlns='urn:oasis:names:tc:opendocument:xmlns:container'>
                  <rootfiles>
                    <rootfile full-path='OEBPS/content.xhtml' media-type='application/xhtml+xml'/>
                  </rootfiles>
                </container>");
            }
        }

        return fileName;
    }

    private string WrapHtml(string body, string title)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'/>
                <title>{title}</title>
            </head>
            <body>
            {body}</body>
            </html>";
    }
}