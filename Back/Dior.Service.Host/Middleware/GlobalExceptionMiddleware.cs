using System.Net;
using System.Text.Json;
using Dior.Library.Exceptions;

namespace Dior.Service.Host.Middleware
{
    /// <summary>
    /// Middleware global de gestion d'erreurs pour standardiser les réponses d'erreur
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur non gérée s'est produite: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var (statusCode, message) = exception switch
            {
                Dior.Library.Exceptions.NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                Dior.Library.Exceptions.UnauthorizedException => (StatusCodes.Status401Unauthorized, "Non autorisé"),
                Dior.Library.Exceptions.ForbiddenException => (StatusCodes.Status403Forbidden, "Accès interdit"),
                Dior.Library.Exceptions.ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
                Dior.Library.Exceptions.ConflictException => (StatusCodes.Status409Conflict, exception.Message),
                TimeoutException => (StatusCodes.Status408RequestTimeout, "Délai d'attente dépassé"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Paramètres invalides"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Opération invalide"),
                _ => (StatusCodes.Status500InternalServerError, "Une erreur serveur s'est produite")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = new
                {
                    message,
                    type = exception.GetType().Name,
                    statusCode,
                    timestamp = DateTime.UtcNow,
                    path = context.Request.Path
                },
                // Inclure les détails techniques uniquement en développement
                details = _env.IsDevelopment() ? new
                {
                    stackTrace = exception.StackTrace,
                    innerException = exception.InnerException?.Message
                } : null
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}