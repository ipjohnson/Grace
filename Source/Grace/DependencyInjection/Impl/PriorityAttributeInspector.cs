using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Inspects an export strategy and 
	/// </summary>
	public class PriorityAttributeInspector<T> : IExportStrategyInspector where T : Attribute, IExportPriorityAttribute
	{
		/// <summary>
		/// Inspect the strategy for T type attributes
		/// </summary>
		/// <param name="exportStrategy"></param>
		public void Inspect(IExportStrategy exportStrategy)
		{
			IConfigurableExportStrategy configurableExportStrategy = exportStrategy as IConfigurableExportStrategy;

			if (configurableExportStrategy != null &&
				 configurableExportStrategy.Attributes != null)
			{
				IExportPriorityAttribute attribute =
					configurableExportStrategy.Attributes.FirstOrDefault(x => x is T) as IExportPriorityAttribute;

				if (attribute != null)
				{
					int newPriority =
						attribute.ProvidePriority(configurableExportStrategy.ActivationType);

					configurableExportStrategy.SetPriority(newPriority);
				}
			}
		}
	}
}
