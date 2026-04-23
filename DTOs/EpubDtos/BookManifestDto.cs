namespace OpenBooksBackMobile.DTOs.EpubDtos
{
    public class BookManifestDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }

        public List<ReadingOrderItem> ReadingOrder { get; set; } = new();
        public List<ResourceItem> Resources { get; set; } = new();
        public List<TocLinkDto> Toc { get; set; } = new();
    }
}
