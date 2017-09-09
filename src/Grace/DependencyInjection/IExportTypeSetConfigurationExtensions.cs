using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// extension methods for type set configuration
    /// </summary>
    public static class IExportTypeSetConfigurationExtensions
    {
        /// <summary>
        /// Ups the priority of partially closed generics based on the number of closed parameters
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <returns>configuration object</returns>
        public static IExportTypeSetConfiguration PrioritizePartiallyClosedGenerics(
            this IExportTypeSetConfiguration configuration)
        {
            configuration.WithInspector(new PartiallyClosedGenericPriorityAugmenter());

            return configuration;
        }
    }
}
