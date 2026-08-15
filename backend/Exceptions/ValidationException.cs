namespace EmployeeTaskManagement.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, List<string>> Errors { get; set; } = new();

        public ValidationException(string message, Dictionary<string, List<string>>? errors = null)
            : base(message, 400, "VALIDATION_ERROR")
        {
            if (errors != null)
            {
                Errors = errors;
            }
        }
    }
}
