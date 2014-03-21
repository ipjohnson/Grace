using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Increases the priority of partially closed generics
	/// </summary>
	public class PartiallyClosedGenericPriorityAugmenter : IExportStrategyInspector
	{
		/// <summary>
		/// increases the strategies priority based on the number of closed parameters it has
		/// </summary>
		/// <param name="exportStrategy"></param>
		public void Inspect(IExportStrategy exportStrategy)
		{
			IConfigurableExportStrategy configurableExport = exportStrategy as IConfigurableExportStrategy;
			
			if (configurableExport != null && exportStrategy.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
			{
				int maxClosed = 0;
				int openGenericParams = exportStrategy.ActivationType.GetTypeInfo().GenericTypeParameters.Length;

				foreach (Type exportType in exportStrategy.ExportTypes)
				{
					int exportTypeParamCount = exportType.GetTypeInfo().GenericTypeParameters.Length;
					int numClosed = exportTypeParamCount - openGenericParams;

					if (numClosed > maxClosed)
					{
						maxClosed = numClosed;
					}
				}

				configurableExport.SetPriority(configurableExport.Priority + maxClosed);
			}
		}
	}
}
