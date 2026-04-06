namespace PilotIceService.WebApi.Mappers
{
    public class RequestStatus
    {
        /// <summary>
        /// Id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Тип статуса.
        /// </summary>
        public string? StatusTypeName { get; set; }

        /// <summary>
        /// Причина.
        /// </summary>
        public string? Reason { get; set; }
    }
}