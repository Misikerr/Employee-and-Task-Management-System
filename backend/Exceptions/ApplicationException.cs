namespace EmployeeTaskManagement.Exceptions
{
    public class ApplicationException : Exception
    {
        public int StatusCode { get; set; }
        public string? ErrorCode { get; set; }

        public ApplicationException(string message, int statusCode = 500, string? errorCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
