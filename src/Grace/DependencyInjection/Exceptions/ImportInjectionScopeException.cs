namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception is thrown when trying to import IInjectionScope. Under most circumstances you want to inject IExportLocatorScope
    /// </summary>
    public class ImportInjectionScopeException : LocateException
    {
        /// <summary>
        /// Error message for exception
        /// </summary>
        public const string ErrorMessage = "IInjectionScope was requested, please change it to IExportLocatorScope.";

        /// <summary>
        /// Default constructor takes context
        /// </summary>
        /// <param name="context">static injection context</param>
        public ImportInjectionScopeException(StaticInjectionContext context) : base(context, ErrorMessage)
        {

        }
    }
}
