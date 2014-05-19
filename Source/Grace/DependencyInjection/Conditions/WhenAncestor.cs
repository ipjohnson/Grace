using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// When ancestor is of certain type
	/// </summary>
	public class WhenAncestor : IExportCondition
	{
		private readonly Type[] ancestorTypes;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="ancestorTypes">ancestor types</param>
		public WhenAncestor(params Type[] ancestorTypes)
		{
			this.ancestorTypes = ancestorTypes;
		}

		/// <summary>
		/// Condition meet
		/// </summary>
		/// <param name="scope">injection scope</param>
		/// <param name="injectionContext">injection context</param>
		/// <param name="exportStrategy">export strategy</param>
		/// <returns>condition</returns>
		public bool ConditionMeet(IInjectionScope scope, IInjectionContext injectionContext, IExportStrategy exportStrategy)
		{
			bool found = false;
			CurrentInjectionInfo[] injectionStack = injectionContext.GetInjectionStack();

			for (int i = injectionStack.Length - 1; i >= 0 && !found; i--)
			{
				CurrentInjectionInfo injectionInfo = injectionStack[i];

				foreach (Type ancestorType in ancestorTypes)
				{
					if (ancestorType.GetTypeInfo().IsInterface)
					{
						if(injectionInfo.ActivationType == ancestorType ||
							(injectionInfo.CurrentExportStrategy != null && injectionInfo.CurrentExportStrategy.ExportTypes.Contains(ancestorType)))
						{
							found = true;
							break;
						}
					}
					else if (injectionInfo.ActivationType == ancestorType)
					{
						found = true;
						break;
					}
				}
			}

			return found;
		}
	}
}
