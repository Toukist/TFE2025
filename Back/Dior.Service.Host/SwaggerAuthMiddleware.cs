using System.Text;

namespace Dior.Service.Host.Extensions
{
    public class SwaggerAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
                {
                    var encoded = authHeader.Substring("Basic ".Length).Trim();
                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    var parts = credentials.Split(':', 2);

                    if (parts.Length == 2)
                    {
                        var username = parts[0];
                        var password = parts[1];

                        if (username == "admin" && password == "admin2024")
                        {
                            await _next(context);
                            return;
                        }
                    }
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger Documentation\"";
                await context.Response.WriteAsync("Accès non autorisé");
                return;
            }

            await _next(context);
        }
    }

    public static class SwaggerAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthMiddleware>();
        }
    }
}
