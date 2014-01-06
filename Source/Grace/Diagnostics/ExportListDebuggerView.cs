using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
	[DebuggerDisplay("{DisplayValue,nq}",Name = "{NameDisplayValue,nq}")]
	public class ExportListDebuggerView
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string name;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<IExportStrategy> strategies;
 
		public ExportListDebuggerView(string name)
		{
			this.name = name;
			strategies = new List<IExportStrategy>();
		}

		public void Add(IExportStrategy strategy)
		{
			strategies.Add(strategy);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public IEnumerable<IExportStrategy> Strategies
		{
			get { return strategies; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DisplayValue
		{
			get
			{
				string returnString = null;

				foreach (IExportStrategy exportStrategy in Strategies)
				{
					if (returnString == null)
					{
						returnString = exportStrategy.ActivationType.FullName;
					}
					else
					{
						returnString += " ...";

						break;
					}
				}

				return returnString;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string NameDisplayValue
		{
			get { return name + " "; }
		}
	}
}
