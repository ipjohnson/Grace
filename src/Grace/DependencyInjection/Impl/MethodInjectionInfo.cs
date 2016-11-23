using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// information for injecting a method
    /// </summary>
    public class MethodInjectionInfo
    {
        /// <summary>
        /// Method being injected
        /// </summary>
        public MethodInfo Method { get; set; }
    }
}
