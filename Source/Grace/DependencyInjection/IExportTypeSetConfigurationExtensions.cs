using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
