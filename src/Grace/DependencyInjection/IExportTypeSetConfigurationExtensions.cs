using System;
using System.Reflection;
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

        /// <summary>
        /// Export a type set by a keyed interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="keyFunc"></param>
        /// <returns></returns>
        public static IExportTypeSetConfiguration ByKeyed<T>(this IExportTypeSetConfiguration configuration, Func<Type, object> keyFunc)
        {
            configuration.ByKeyedTypes(type =>
            {
                if (typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                {
                    var key = keyFunc(type);

                    if (key != null)
                    {
                        return new[] { new Tuple<Type, object>(type, key) };
                    }

                    return null;
                }

                return null;
            });

            return configuration;
        }
    }
}
