namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключение при конфликтах с настройками приложения.
    /// </summary>
    public class ApplicationSettingsException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsException" /> class.
        /// </summary>
        public ApplicationSettingsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public ApplicationSettingsException(Exception innerException)
            : base(innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public ApplicationSettingsException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public ApplicationSettingsException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_ApplicationSettings";

        public override string Title { get; set; } = "Ограничение настроек приложения!";
    }
}
