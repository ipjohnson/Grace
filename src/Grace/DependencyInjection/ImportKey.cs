namespace Grace.DependencyInjection
{
    /// <summary>
    /// Special keys with built-in Grace purpose
    /// </summary>
    public static class ImportKey
    {
        /// <summary>
        /// Key used to inject the currently located key.
        /// Typically used into a service that is exported as ImportKey.Any.
        /// </summary>
        public static readonly object Key = new object();
    }
}