using System.Collections;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PilotIceService.Application.Exceptions;

namespace PilotIceService.WebApi.Middleware.GlobalExceptions
{
    /// <summary>
    /// Middleware for handling global exceptions.
    /// </summary>
    public class GlobalExceptionsMiddleware
    {
        private readonly ILogger<GlobalExceptionsMiddleware> _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionsMiddleware" /> class.
        /// </summary>
        /// <param name="next"><see cref="RequestDelegate" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public GlobalExceptionsMiddleware(RequestDelegate next, ILogger<GlobalExceptionsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoke.
        /// </summary>
        /// <param name="httpContext"><see cref="HttpContext" />.</param>
        /// <param name="problemDetailsFactory"><see cref="ProblemDetailsFactory" />.</param>
        /// <param name="environment"><see cref="IWebHostEnvironment" />.</param>
        public async Task Invoke(
            HttpContext httpContext,
            ProblemDetailsFactory problemDetailsFactory,
            IWebHostEnvironment environment)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                try
                {
                    var error = GetResponseErrorDetails(ex, httpContext, problemDetailsFactory);

                    if (environment.IsDevelopment())
                    {
                        error.ExceptionType = ex.GetType().Name;
                        error.StackTrace = ex.StackTrace?.Split("\r\n");
                    }

                    var result = JsonSerializer.Serialize(error, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    });

                    var response = httpContext.Response;

                    if (response.HasStarted)
                    {
                        _logger.LogError(ex, "Error: exception outside scope!");
                    }
                    else
                    {
                        response.StatusCode = error.Status!.Value;
                        response.ContentType = "application/json";
                        if (response.StatusCode >= 500)
                        {
                            _logger.LogError(ex, "Error: exception outside scope!");
                        }
                        else
                        {
                            _logger.LogWarning(error.Title, error.Data);
                        }
                    }

                    await response.WriteAsync(result);
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "Error in global exception filter!");
                }
            }
        }

        private ResponseErrorDetailsForDeveloper GetResponseErrorDetails(
            Exception ex,
            HttpContext httpContext,
            ProblemDetailsFactory problemDetailsFactory)
        {
            HttpStatusCode status;
            string exceptionKey;
            string title;
            ICollection? data = null;

            if (ex is AggregateException && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            switch (ex)
            {
                case ArgumentException _:
                    {
                        title = ex.Message;
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.BadRequest;
                    }

                    break;

                case ApplicationSettingsException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = currentEx.Message;
                        status = HttpStatusCode.BadRequest;
                    }

                    break;

                case UnauthorizedAccessException _:
                    {
                        title = "Не валидные данные!";
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.Unauthorized;
                    }

                    break;

                case NotAllowException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.Forbidden;
                    }

                    break;

                case NotFoundException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.NotFound;
                    }

                    break;
                case AlreadyExistsException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.Conflict;
                    }

                    break;
                case ToManyRequestsException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.TooManyRequests;
                    }

                    break;

                case OperationCanceledException currentEx:
                    {
                        title = currentEx.Message;
                        exceptionKey = currentEx.Message;
                        status = (HttpStatusCode)499;
                    }

                    break;

                case InternalIntegrationException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = ex.Message;
                        status = currentEx?.InternalError.Status != null
                        ? (HttpStatusCode)currentEx?.InternalError.Status
                        : HttpStatusCode.BadRequest;
                    }

                    break;

                case ExternalIntegrationException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = currentEx.Message;
                        status = HttpStatusCode.BadRequest;
                    }

                    break;
                case AppException currentEx:
                    {
                        title = currentEx.Title;
                        exceptionKey = ex.Message;
                        status = HttpStatusCode.BadRequest;
                    }

                    break;

                default:
                    {
                        title = "Internal server error!";
                        exceptionKey = "InternalServerError (report to a program administrator)";
                        status = HttpStatusCode.InternalServerError;
                    }

                    break;
            }

            var problem = problemDetailsFactory.CreateProblemDetails(httpContext, (int)status);

            problem.Extensions.TryGetValue("traceId", out var traceId);

            ResponseErrorDetailsForDeveloper result = new()
            {
                Title = title,
                Detail = exceptionKey,
                Status = problem.Status,
                Type = problem.Type,
                TraceId = traceId as string,
                Data = ex.Data,
            };

            return result;
        }
    }
}
