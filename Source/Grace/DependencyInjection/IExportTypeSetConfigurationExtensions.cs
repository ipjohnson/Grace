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
		/// Ups the priority of partially closed generics based on the number of closed parameters
		/// </summary>
		/// <param name="configuration"></param>
		/// <returns></returns>
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
