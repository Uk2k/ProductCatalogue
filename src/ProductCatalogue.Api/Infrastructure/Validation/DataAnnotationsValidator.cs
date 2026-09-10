using System.ComponentModel.DataAnnotations;

namespace ProductCatalogue.Api.Infrastructure.Validation;

public static class DataAnnotationsValidator
{
    public static IDictionary<string, string[]> Validate<T>(T model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model!);
        Validator.TryValidateObject(model!, context, validationResults, validateAllProperties: true);

        return validationResults
            .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty)
                .Select(member => new { member, message = result.ErrorMessage ?? "The value is invalid." }))
            .GroupBy(error => error.member)
            .ToDictionary(group => group.Key, group => group.Select(error => error.message).Distinct().ToArray());
    }
}
