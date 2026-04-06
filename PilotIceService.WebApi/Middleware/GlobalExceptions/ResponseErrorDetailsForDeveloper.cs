using System.Text.Json.Serialization;

namespace PilotIceService.WebApi.Middleware.GlobalExceptions
{
    public class ResponseErrorDetailsForDeveloper : ResponseErrorDetails
    {
        [JsonPropertyName("exceptionType")]
        public string? ExceptionType { get; set; }

        [JsonPropertyName("stackTrace")]
        public string[]? StackTrace { get; set; }
    }
}