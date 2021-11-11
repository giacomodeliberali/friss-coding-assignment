namespace Application.Contracts
{
    /// <summary>
    /// Exceptions will be serialized into this model prior to returning to client.
    /// </summary>
    public record ExceptionDto
    {
        /// <summary>
        /// The name of the thrown exception.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The (possibly null) stackTrace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// The exception message.
        /// </summary>
        public string Message { get; set; }
    }
}
