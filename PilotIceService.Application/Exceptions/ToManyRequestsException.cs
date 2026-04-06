namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключении при запросе несуществующей сущности.
    /// </summary>
    public class ToManyRequestsException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToManyRequestsException" /> class.
        /// </summary>
        public ToManyRequestsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToManyRequestsException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public ToManyRequestsException(Exception innerException)
            : base(innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToManyRequestsException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public ToManyRequestsException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToManyRequestsException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public ToManyRequestsException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_TooManyRequests";

        public override string Title { get; set; } = "Превышено количество запросов!";
    }
}
