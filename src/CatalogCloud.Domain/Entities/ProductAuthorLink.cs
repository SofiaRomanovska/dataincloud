using System.Text.Json.Serialization;

namespace CatalogCloud.Domain.Entities;

public class ProductAuthorLink
{
    public const int MaxAuthorIdLength = 128;

    private ProductAuthorLink()
    {
    }

    public ProductAuthorLink(Guid productId, string authorId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(productId));
        }

        ProductId = productId;
        ChangeAuthor(authorId);
    }

    [JsonInclude]
    public Guid ProductId { get; private set; }

    [JsonInclude]
    public string AuthorId { get; private set; } = string.Empty;

    [JsonInclude]
    public DateTime CachedAt { get; private set; }

    public void ChangeAuthor(string authorId)
    {
        if (string.IsNullOrWhiteSpace(authorId))
        {
            throw new ArgumentException("Author id is required.", nameof(authorId));
        }

        if (authorId.Length > MaxAuthorIdLength)
        {
            throw new ArgumentException($"Author id cannot exceed {MaxAuthorIdLength} characters.", nameof(authorId));
        }

        AuthorId = authorId;
        CachedAt = DateTime.UtcNow;
    }
}
