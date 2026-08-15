namespace EmployeeTaskManagement.Exceptions
{
    public class ConflictException : ApplicationException
    {
        public ConflictException(string message)
            : base(message, 409, "CONFLICT")
        {
        }
    }
}
