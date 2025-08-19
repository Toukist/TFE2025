using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Dior.Service.Host.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
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
                _logger.LogError(ex, "Une exception non gérée s'est produite");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = context.TraceIdentifier }
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    problemDetails.Title = "Accès non autorisé";
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Detail = "Vous n'avez pas l'autorisation d'accéder à cette ressource";
                    break;

                case ArgumentException argEx:
                    problemDetails.Title = "Argument invalide";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = argEx.Message;
                    break;

                case KeyNotFoundException:
                    problemDetails.Title = "Ressource introuvable";
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Detail = "La ressource demandée n'a pas été trouvée";
                    break;

                case InvalidOperationException invOpEx:
                    problemDetails.Title = "Opération invalide";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = invOpEx.Message;
                    break;

                case TimeoutException:
                    problemDetails.Title = "Délai d'attente dépassé";
                    problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
                    problemDetails.Detail = "L'opération a pris trop de temps à s'exécuter";
                    break;

                default:
                    problemDetails.Title = "Erreur interne du serveur";
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;

                    if (_env.IsDevelopment())
                    {
                        problemDetails.Detail = exception.Message;
                        problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
                    }
                    else
                    {
                        problemDetails.Detail = "Une erreur inattendue s'est produite";
                    }
                    break;
            }

            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var response = JsonSerializer.Serialize(problemDetails, jsonOptions);
            await context.Response.WriteAsync(response);
        }
    }
}