using Bookstore.Application.DTOs;
using Bookstore.Application.Validators;
using FluentValidation.TestHelper;

namespace Bookstore.Tests.Unit;

public class AuthorValidatorsTests
{
    private readonly CreateAuthorDtoValidator _createValidator;
    private readonly UpdateAuthorDtoValidator _updateValidator;

    public AuthorValidatorsTests()
    {
        _createValidator = new CreateAuthorDtoValidator();
        _updateValidator = new UpdateAuthorDtoValidator();
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenFullNameIsEmpty()
    {
        var dto = CreateValidAuthorDto();
        dto.FullName = string.Empty;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenBiographyIsTooShort()
    {
        var dto = CreateValidAuthorDto();
        dto.Biography = "Too short";

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Biography);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenBirthDateIsInFuture()
    {
        var dto = CreateValidAuthorDto();
        dto.BirthDate = DateTime.UtcNow.AddDays(1);

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.BirthDate);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenPublishedBooksCountIsNegative()
    {
        var dto = CreateValidAuthorDto();
        dto.PublishedBooksCount = -1;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.PublishedBooksCount);
    }

    [Fact]
    public void CreateValidator_ShouldHaveError_WhenIsActiveIsMissing()
    {
        var dto = CreateValidAuthorDto();
        dto.IsActive = null;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.IsActive);
    }

    [Fact]
    public void UpdateValidator_ShouldNotHaveError_WhenPayloadIsValid()
    {
        var dto = new UpdateAuthorDto
        {
            FullName = "Valid Author",
            Biography = "A valid biography that satisfies all restrictions.",
            BirthDate = new DateTime(1990, 5, 5),
            PublishedBooksCount = 10,
            IsActive = false
        };

        var result = _updateValidator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateAuthorDto CreateValidAuthorDto()
    {
        return new CreateAuthorDto
        {
            FullName = "Valid Author",
            Biography = "A valid biography that satisfies all restrictions.",
            BirthDate = new DateTime(1990, 5, 5),
            PublishedBooksCount = 10,
            IsActive = true
        };
    }
}
