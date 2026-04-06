namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключении при наличии сущности.
    /// </summary>
    public class AlreadyExistsException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException" /> class.
        /// </summary>
        public AlreadyExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public AlreadyExistsException(Exception innerException)
            : base(innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public AlreadyExistsException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyExistsException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public AlreadyExistsException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_AlreadyExists";

        public override string Title { get; set; } = "Объект уже существует!";
    }
}
