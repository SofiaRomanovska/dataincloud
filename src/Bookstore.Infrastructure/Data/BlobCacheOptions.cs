namespace Bookstore.Infrastructure.Data;

public class BlobCacheOptions
{
    public const string SectionName = "BlobCache";

    public string ContainerPath { get; set; } = "blob-cache";
    public string ProductAuthorLinksBlobName { get; set; } = "product-author-links.json";
}
