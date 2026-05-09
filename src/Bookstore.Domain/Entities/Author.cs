namespace Bookstore.Domain.Entities;

public class Author
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int PublishedBooksCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
