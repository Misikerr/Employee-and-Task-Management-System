using Microsoft.AspNetCore.Authorization;

namespace EmployeeTaskManagement.Authorization
{
    public static class AuthorizationExtensions
    {
        public static AuthorizationPolicyBuilder RequireRoleNames(this AuthorizationPolicyBuilder builder, params string[] roles)
        {
            return builder.RequireAssertion(context =>
            {
                var userRoles = context.User.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                return roles.Any(role => userRoles.Contains(role));
            });
        }
    }
}
