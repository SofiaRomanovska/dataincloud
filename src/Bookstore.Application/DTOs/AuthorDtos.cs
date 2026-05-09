namespace Bookstore.Application.DTOs;

public class AuthorDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int PublishedBooksCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAuthorDto
{
    public string FullName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int PublishedBooksCount { get; set; }
    public bool? IsActive { get; set; } = true;
}

public class UpdateAuthorDto
{
    public string FullName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int PublishedBooksCount { get; set; }
    public bool? IsActive { get; set; }
}
