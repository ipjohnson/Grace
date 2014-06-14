using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// extension methods for export type set configuration
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public static class IExportTypeSetConfigurationExtensions
	{
        /// <summary>
        /// Adds an inspector action that can be used to configure all exports
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="inspector">inspector action</param>
        /// <returns>configuration object</returns>
	    public static IExportTypeSetConfiguration WithInspector(this IExportTypeSetConfiguration configuration,
	        Action<IExportStrategy> inspector)
	    {
	        configuration.WithInspector(new FuncExportStrategyInspector(inspector));

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
