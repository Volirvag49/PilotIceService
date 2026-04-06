namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключении при запросе недоступной сущности.
    /// </summary>
    public class NotAllowException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAllowException" /> class.
        /// </summary>
        public NotAllowException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAllowException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public NotAllowException(Exception innerException)
            : base(innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAllowException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public NotAllowException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAllowException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public NotAllowException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_NotAllowed";

        public override string Title { get; set; } = "Нет доступа к запрашиваемому объекту!";
    }
}
