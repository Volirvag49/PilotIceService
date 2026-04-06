namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключении приложения.
    /// </summary>
    public class AppException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        public AppException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public AppException(Exception innerException, string? messageKey = null)
            : base(null, innerException)
        {
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public AppException(string details, string? messageKey = null)
            : base(details)
        {
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public AppException(string details, Exception innerException, string? messageKey = null)
            : base(details, innerException)
        {
            MessageKey = messageKey;
        }

        public virtual string TitleKey => "ExceptionTitle_App";

        public virtual string Title { get; set; } = "Внутренняя ошибка приложения!";

        public string? MessageKey { get; set; }
    }
}
