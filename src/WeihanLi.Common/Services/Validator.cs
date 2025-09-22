using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WeihanLi.Extensions;
using AnnotationValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using ValidationResult = WeihanLi.Common.Models.ValidationResult;

namespace WeihanLi.Common.Services;

public interface IValidator
{
    [RequiresUnreferencedCode("Unreferenced code may be used.")]
    ValidationResult Validate(object? value);
}

public interface IValidator<in T>
{
    ValidationResult Validate(T value);
}

public interface IAsyncValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T value);
}

public sealed class DataAnnotationValidator : IValidator
{
    public static IValidator Instance { get; } = new DataAnnotationValidator();

    [RequiresUnreferencedCode("Unreferenced code may be used.")]
    public ValidationResult Validate(object? value)
    {
        var validationResult = new ValidationResult();
        if (value is null)
        {
            validationResult.Valid = false;
            validationResult.Errors ??= [];
            validationResult.Errors[string.Empty] = ["Value is null"];
        }
        else
        {
            var annotationValidateResults = new List<AnnotationValidationResult>();
            validationResult.Valid =
                Validator.TryValidateObject(value, new ValidationContext(value), annotationValidateResults);
            validationResult.Errors = annotationValidateResults
                .GroupBy(x => x.MemberNames.StringJoin(","))
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).WhereNotNull().ToArray());
        }
        return validationResult;
    }
}

public sealed class DelegateValidator(Func<object?, ValidationResult> validateFunc) : IValidator
{
    private readonly Func<object?, ValidationResult> _validateFunc = Guard.NotNull(validateFunc);

    public ValidationResult Validate(object? value)
    {
        return _validateFunc.Invoke(value);
    }
}

public sealed class DelegateValidator<T> : IValidator<T>, IAsyncValidator<T>
{
    private readonly Func<T, Task<ValidationResult>> _validateFunc;

    public DelegateValidator(Func<T, ValidationResult> validateFunc)
    {
        Guard.NotNull(validateFunc);
        _validateFunc = t => validateFunc(t).WrapTask();
    }

    public DelegateValidator(Func<T, Task<ValidationResult>> validateFunc)
    {
        _validateFunc = Guard.NotNull(validateFunc);
    }

    public ValidationResult Validate(T value)
    {
        return _validateFunc.Invoke(value).GetAwaiter().GetResult();
    }

    public Task<ValidationResult> ValidateAsync(T value)
    {
        return _validateFunc.Invoke(value);
    }
}
