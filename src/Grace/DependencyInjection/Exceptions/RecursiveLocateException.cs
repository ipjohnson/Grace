namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// Exception thrown when a recursive loop is detected in the object graph
    /// </summary>
    public class RecursiveLocateException : LocateException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"></param>
        public RecursiveLocateException(StaticInjectionContext context) : base(context, "Recursive object graph detected")
        {
        }
    }
}
