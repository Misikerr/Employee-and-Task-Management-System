namespace EmployeeTaskManagement.Authorization
{
    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string ManagerOrAdmin = "ManagerOrAdmin";
        public const string EmployeeOrManagerOrAdmin = "EmployeeOrManagerOrAdmin";
    }
}
