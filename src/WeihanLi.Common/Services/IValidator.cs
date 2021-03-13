using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeihanLi.Common.Services
{
    public interface IValidator
    {
        bool TryValidate(object value, out ICollection<ValidationResult> validationResults);
    }

    public class DataAnnotationValidator : IValidator
    {
        public virtual bool TryValidate(object value, out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(value, new ValidationContext(value), validationResults);
        }
    }
}
