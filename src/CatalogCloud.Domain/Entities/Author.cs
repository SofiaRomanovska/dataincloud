namespace CatalogCloud.Domain.Entities;

public class Author
{
    public const int MaxFullNameLength = 160;
    public const int MinBiographyLength = 20;
    public const int MaxBiographyLength = 2_000;
    public const int MaxPublishedBooksCount = 10_000;

    private static readonly DateTime EarliestBirthDate = new(1800, 1, 1);

    private Author()
    {
    }

    public Author(string fullName, string biography, DateTime birthDate, int publishedBooksCount, bool isActive)
    {
        Id = Guid.NewGuid().ToString("N");
        CreatedAt = DateTime.UtcNow;

        UpdateDetails(fullName, biography, birthDate, publishedBooksCount, isActive);
    }

    public string Id { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Biography { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }
    public int PublishedBooksCount { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static DateTime MinimumBirthDate => EarliestBirthDate;

    public void UpdateDetails(string fullName, string biography, DateTime birthDate, int publishedBooksCount, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Author full name is required.", nameof(fullName));
        }

        if (fullName.Length > MaxFullNameLength)
        {
            throw new ArgumentException($"Author full name cannot exceed {MaxFullNameLength} characters.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(biography))
        {
            throw new ArgumentException("Author biography is required.", nameof(biography));
        }

        if (biography.Length < MinBiographyLength)
        {
            throw new ArgumentException($"Author biography must contain at least {MinBiographyLength} characters.", nameof(biography));
        }

        if (biography.Length > MaxBiographyLength)
        {
            throw new ArgumentException($"Author biography cannot exceed {MaxBiographyLength} characters.", nameof(biography));
        }

        if (birthDate.Date <= EarliestBirthDate)
        {
            throw new ArgumentOutOfRangeException(nameof(birthDate), birthDate, "Author birth date is too early.");
        }

        if (birthDate.Date >= DateTime.UtcNow.Date)
        {
            throw new ArgumentOutOfRangeException(nameof(birthDate), birthDate, "Author birth date must be in the past.");
        }

        if (publishedBooksCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(publishedBooksCount), publishedBooksCount, "Published books count cannot be negative.");
        }

        if (publishedBooksCount > MaxPublishedBooksCount)
        {
            throw new ArgumentOutOfRangeException(nameof(publishedBooksCount), publishedBooksCount, $"Published books count cannot exceed {MaxPublishedBooksCount}.");
        }

        FullName = fullName;
        Biography = biography;
        BirthDate = birthDate;
        PublishedBooksCount = publishedBooksCount;
        IsActive = isActive;
    }
}
