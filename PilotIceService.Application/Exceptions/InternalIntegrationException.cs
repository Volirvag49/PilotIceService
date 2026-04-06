namespace PilotIceService.Application.Exceptions
{
    public class InternalIntegrationException : AppException
    {
        public InternalIntegrationException(RestClientInternalErrorResponse internalResponseError)
        {
            InternalError = internalResponseError;

            if (internalResponseError != null)
            {
                this.WithData("internalTraceId", internalResponseError.TraceId);

                if (internalResponseError.Data != null)
                {
                    this.WithData("internalData", internalResponseError.Data);
                }
            }
        }

        public override string TitleKey => "ExceptionTitle_InternalIntegration";

        public override string Title { get; set; } = "Внутренняя интеграционная ошибка!";

        public RestClientInternalErrorResponse InternalError { get; set; }
    }
}
