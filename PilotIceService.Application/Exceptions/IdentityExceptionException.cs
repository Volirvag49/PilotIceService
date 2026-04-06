namespace PilotIceService.Application.Exceptions
{
    /// <summary>
    /// Исключение аутентификации и авторизации.
    /// </summary>
    public class IdentityExceptionException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityExceptionException" /> class.
        /// </summary>
        public IdentityExceptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityExceptionException" /> class.
        /// </summary>
        /// <param name="innerException">Исключение.</param>
        public IdentityExceptionException(Exception innerException)
            : base(innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityExceptionException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        public IdentityExceptionException(string details)
            : base(details)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityExceptionException" /> class.
        /// </summary>
        /// <param name="details">Дополнительная информация.</param>
        /// <param name="innerException">Исключение.</param>
        public IdentityExceptionException(string details, Exception innerException)
            : base(details, innerException)
        {
        }

        public override string TitleKey => "ExceptionTitle_Identity";

        public override string Title { get; set; } = "Ошибка аутентификации и авторизации!";
    }
}
