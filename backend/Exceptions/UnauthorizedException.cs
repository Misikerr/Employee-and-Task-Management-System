namespace EmployeeTaskManagement.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(message, 401, "UNAUTHORIZED")
        {
        }
    }
}
