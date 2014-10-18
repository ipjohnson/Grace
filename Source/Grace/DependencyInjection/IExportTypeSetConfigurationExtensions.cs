using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Data;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// extension methods for export type set configuration
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IExportTypeSetConfigurationExtensions
	{
        /// <summary>
        /// Prioritize specfic types that
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="typesThat">types that match func</param>
        /// <param name="priority">priority</param>
        /// <returns>configuration object</returns>
	    public static IExportTypeSetConfiguration Prioritize(this IExportTypeSetConfiguration configuration,Func<Type, bool> typesThat, int priority = 1)
        {
            configuration.WithInspector(new PrioritizeTypesThatInspector(typesThat, priority));

            return configuration;
        }

        /// <summary>
        /// Configures exports lifestyles based on class name. If a class ends in Singleton or Service will be registered as Singleton,
        /// classes ending in Scoped, ScopedSingleton, or ScopedService will be registered as Scoped Singleton. Everything else will be transient
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IExportTypeSetConfiguration UsingLifestyleConventions(this IExportTypeSetConfiguration configuration)
        {
            configuration.UsingLifestyle(
                t =>
                {
                    string lowerName = t.Name.ToLower();

                    if (lowerName.EndsWith("service") || 
                        lowerName.EndsWith("singleton"))
                    {
                        return new SingletonLifestyle();
                    }

                    if (lowerName.EndsWith("scoped") || 
                        lowerName.EndsWith("scopedsingleton") ||
                        lowerName.EndsWith("scopedservice"))
                    {
                        return new SingletonPerScopeLifestyle();
                    }
                                             
                    return null;
                });

            return configuration;
        }

        /// <summary>
        /// Adds an inspector action that can be used to configure all exports
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="inspector">inspector action</param>
        /// <returns>configuration object</returns>
	    public static IExportTypeSetConfiguration WithInspector(this IExportTypeSetConfiguration configuration,
            Action<ICompiledExportStrategy> inspector)
	    {
	        configuration.WithInspector(
                new FuncExportStrategyInspector(
                    strategy =>
                    {
                        ICompiledExportStrategy compiledExportStrategy =
                                                     strategy as ICompiledExportStrategy;

                        if (compiledExportStrategy != null)
                        {
                            inspector(compiledExportStrategy);
                        }
                    }));

	        return configuration;
	    }

        /// <summary>
        /// Adds an inspector action for strategies that export the specified type, can be class or interface
        /// </summary>
        /// <typeparam name="T">type to inspect</typeparam>
        /// <param name="configuration">configuration object</param>
        /// <param name="inspector">inspector</param>
        /// <returns>configuration object</returns>
	    public static IExportTypeSetConfiguration WithInspectorFor<T>(this IExportTypeSetConfiguration configuration,
            Action<ICompiledExportStrategy> inspector)
	    {
	        Action<IExportStrategy> filter = 
                strategy =>
	            {
	                ICompiledExportStrategy compiledExportStrategy = strategy as ICompiledExportStrategy;

                    if (compiledExportStrategy != null &&
                        ReflectionService.CheckTypeIsBasedOnAnotherType(strategy.ActivationType, typeof(T)))
	                {
                        inspector(compiledExportStrategy);
	                }
	            };

	        configuration.WithInspector(new FuncExportStrategyInspector(filter));

	        return configuration;
	    }

        /// <summary>
        /// Adds an inspector action for strategies that export the specified type, can be class or interface
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="inspectType">type to inspect</param>
        /// <param name="inspector">inspector</param>
        /// <returns>configuration object</returns>
        public static IExportTypeSetConfiguration WithInspectorFor(this IExportTypeSetConfiguration configuration, 
            Type inspectType,
            Action<ICompiledExportStrategy> inspector)
        {
            Action<IExportStrategy> filter =
                strategy =>
                {
                    ICompiledExportStrategy compiledExportStrategy = strategy as ICompiledExportStrategy;

                    if (compiledExportStrategy != null &&
                        ReflectionService.CheckTypeIsBasedOnAnotherType(strategy.ActivationType, inspectType))
                    {
                        inspector(compiledExportStrategy);
                    }
                };

            configuration.WithInspector(new FuncExportStrategyInspector(filter));

            return configuration;
        }

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
		/// Process all ICustomEnrichmentExpressionAttribute attributes
		/// </summary>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IExportTypeSetConfiguration ProcessCustomEnrichmentExpressionAttributes(
			this IExportTypeSetConfiguration configuration)
		{
			configuration.EnrichWithExpression(ProcessCustomAttributes);

			return configuration;
		}

		private static IEnumerable<ICustomEnrichmentLinqExpressionProvider> ProcessCustomAttributes(Type exportType)
		{
			foreach (PropertyInfo runtimeProperty in exportType.GetRuntimeProperties())
			{
				if ((runtimeProperty.CanRead && (runtimeProperty.GetMethod.IsStatic || !runtimeProperty.GetMethod.IsPublic)) ||
					 (runtimeProperty.CanWrite && (runtimeProperty.SetMethod.IsStatic || !runtimeProperty.SetMethod.IsPublic)))
				{
					continue;
				}

				foreach (Attribute customAttribute in runtimeProperty.GetCustomAttributes())
				{
					ICustomEnrichmentExpressionAttribute enrichmentAttribute = customAttribute as ICustomEnrichmentExpressionAttribute;

					if (enrichmentAttribute != null)
					{
						var enrichment = enrichmentAttribute.GetProvider(exportType, runtimeProperty);

						if (enrichment != null)
						{
							yield return enrichment;
						}
					}
				}
			}

			foreach (MethodInfo runtimeMethod in exportType.GetRuntimeMethods())
			{
				if (runtimeMethod.IsStatic || !runtimeMethod.IsPublic)
				{
					continue;
				}

				foreach (Attribute customAttribute in runtimeMethod.GetCustomAttributes())
				{
					ICustomEnrichmentExpressionAttribute enrichmentAttribute = customAttribute as ICustomEnrichmentExpressionAttribute;

					if (enrichmentAttribute != null)
					{
						var enrichment = enrichmentAttribute.GetProvider(exportType, runtimeMethod);

						if (enrichment != null)
						{
							yield return enrichment;
						}
					}
				}
			}
		}
	}
}
