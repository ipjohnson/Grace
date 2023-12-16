namespace Grace.DependencyInjection
{
    /// <summary>
    /// Arg helper
    /// </summary>
    public class Arg
    {
        /// <summary>
        /// Any argument of type T
        /// </summary>
        /// <typeparam name="T">type of arg</typeparam>
        /// <returns>default T value</returns>
        public static T Any<T>()
        {
            return default(T);
        }


        /// <summary>
        /// Locate argument of type T
        /// </summary>
        /// <typeparam name="T">type of arg</typeparam>
        /// <returns>default T value</returns>
        public static T Locate<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Locate type and specify how to construct certain dependencies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public static T Locate<T>(object data)
        {
            return default(T);
        }

        /// <summary>
        /// Get the current scope
        /// </summary>
        public static IExportLocatorScope Scope()
        {
            return null;
        }

        /// <summary>
        /// Get the current context
        /// </summary>
        public static IInjectionContext Context()
        {
            return null;
        }
        
        /// <summary>
        /// Get the currently injected key
        /// </summary>
        public static T ImportKey<T>()
        {
            return default;
        }
    }
}
