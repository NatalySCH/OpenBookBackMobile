namespace OpenBooksBackMobile.Services.Interfaces
{
    public interface IPdfToEpubConverter
    {
        Task<string> ConvertAsync(string pdfPath, string outputDirectory, string title);
    }
}
