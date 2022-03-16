using System.ComponentModel.DataAnnotations;
using WeihanLi.Extensions;
using AnnotationValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using ValidationResult = WeihanLi.Common.Models.ValidationResult;

namespace WeihanLi.Common.Services;

public interface IValidator
{
    ValidationResult Validate(object value);
}

public class DataAnnotationValidator : IValidator
{
    public virtual ValidationResult Validate(object value)
    {
        var validationResult = new ValidationResult();
        var annotationValidateResults = new List<AnnotationValidationResult>();
        validationResult.Valid =
            Validator.TryValidateObject(value, new ValidationContext(value), annotationValidateResults);
        validationResult.Errors = annotationValidateResults
            .GroupBy(x => x.MemberNames.StringJoin(","))
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).WhereNotNull().ToArray());
        return validationResult;
    }
}

public sealed class DelegateValidator : IValidator
{
    private readonly Func<object, ValidationResult> _validateFunc;

    public DelegateValidator(Func<object, ValidationResult> validateFunc)
    {
        _validateFunc = Guard.NotNull(validateFunc);
    }

    public ValidationResult Validate(object value)
    {
        return _validateFunc.Invoke(value);
    }
}
