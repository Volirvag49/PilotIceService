namespace PilotIceService.Application.Exceptions
{
    public class ExternalIntegrationException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        public ExternalIntegrationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public ExternalIntegrationException(Exception innerException)
            : base(null, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public ExternalIntegrationException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public ExternalIntegrationException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_ExternalIntegration";

        public override string Title { get; set; } = "Внешняя интеграционная ошибка!";
    }
}
