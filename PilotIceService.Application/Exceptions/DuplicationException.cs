namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключении при попытке ввода дублирующихся значений.
    /// </summary>
    public class DuplicationException : AlreadyExistsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException" /> class.
        /// </summary>
        public DuplicationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public DuplicationException(Exception innerException)
            : base(innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public DuplicationException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicationException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public DuplicationException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_Duplication";

        public override string Title { get; set; } = "Дублирование ключевых значений!";
    }
}
