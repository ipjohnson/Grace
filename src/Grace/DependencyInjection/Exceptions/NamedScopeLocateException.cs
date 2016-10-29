namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// Exception thrown 
    /// </summary>
    public class NamedScopeLocateException : LocateException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <param name="context">static injection context</param>
        public NamedScopeLocateException(string scopeName, StaticInjectionContext context) : base(context, $"Could not locate scope with name {scopeName}")
        {
            ScopeName = scopeName;
        }
        
        /// <summary>
        /// Scope name that couldn't be found
        /// </summary>
        public string ScopeName { get; }
    }
}
