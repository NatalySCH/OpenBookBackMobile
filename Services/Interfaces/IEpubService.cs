using OpenBooksBackMobile.DTOs.EpubDtos;

namespace OpenBooksBackMobile.Services.Interfaces
{
    public interface IEpubService
    {
        Task<BookManifestDto> GetManifestAsync(int libroId);
        Task<(byte[] Data, string ContentType)> GetResourceAsync(int libroId, string path);
    }
}
