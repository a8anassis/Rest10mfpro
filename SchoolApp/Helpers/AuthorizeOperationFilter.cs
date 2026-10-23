using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SchoolApp.Helpers
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();

            if (authAttributes.Any())
            {
                operation.Responses!.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses!.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

                var roles = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Where(attr => !string.IsNullOrEmpty(attr.Roles))
                    .SelectMany(attr => attr.Roles!.Split(','))
                    .Select(r => r.Trim());

                // Add the security requirement for JWT Bearer with specified roles
                operation.Security ??= [];  // null-coalescing assignment operator - Αναθέτει τη δεξιά τιμή μόνο αν η αριστερή είναι null
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, context.Document)] = roles.ToList()
                });
            }
        }
    }
}
