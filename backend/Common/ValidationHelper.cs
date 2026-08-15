using System.ComponentModel.DataAnnotations;
using AppValidationException = EmployeeTaskManagement.Exceptions.ValidationException;

namespace EmployeeTaskManagement.Common
{
    public static class ValidationHelper
    {
        public static void ValidateObject(object obj)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, context, results, validateAllProperties: true))
            {
                var errors = results
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "General")
                    .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? "Invalid").ToList());

            throw new AppValidationException("One or more validation errors occurred.", errors);
            }
        }

        public static void ThrowIfNullOrWhitespace(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { fieldName, new List<string> { $"{fieldName} is required." } }
                };
                throw new AppValidationException("Validation failed.", errors);
            }
        }

        public static void ThrowIfInvalid(bool condition, string fieldName, string message)
        {
            if (!condition)
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { fieldName, new List<string> { message } }
                };
                throw new AppValidationException("Validation failed.", errors);
            }
        }
    }
}
