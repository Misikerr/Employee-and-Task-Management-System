namespace EmployeeTaskManagement.Exceptions
{
    public class ResourceNotFoundException : ApplicationException
    {
        public ResourceNotFoundException(string resourceName, string resourceId)
            : base($"{resourceName} with ID {resourceId} was not found.", 404, "RESOURCE_NOT_FOUND")
        {
        }
    }
}
