using System.Text.Json.Serialization;

namespace PilotIceService.WebApi.Middleware.GlobalExceptions
{
    public class ResponseErrorDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }

        [JsonPropertyName("data")]
        public object Data { get; set; }
    }
}