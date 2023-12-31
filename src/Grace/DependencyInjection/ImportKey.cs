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

        /// <summary>
        /// Key matching all requested key during injection.
        /// Exact keys have higher priority than Any keys.
        /// This key meant to be used during registration 
        /// (although it can be located like any other key).
        /// </summary>
        public static readonly object Any = new object();        
    }
}