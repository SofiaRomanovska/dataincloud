namespace Bookstore.Domain.Entities;

public class ProductAuthorLink
{
    public Guid ProductId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
}
