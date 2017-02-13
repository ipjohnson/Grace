using Grace.Data;

namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// Exception thrown when a null value is provided from a factory
    /// </summary>
    public class NullValueProvidedException : LocateException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"></param>
        public NullValueProvidedException(StaticInjectionContext context) : 
            base(context, $"Null value provided from factory for {ReflectionService.GetFriendlyNameForType(context.ActivationType, true)}")
        {
        }

    }
}
