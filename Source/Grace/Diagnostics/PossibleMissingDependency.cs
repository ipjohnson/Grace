using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
	[DebuggerDisplay("{DebuggerDisplayString,nq}", Name = "Missing Dependency")]
	public class PossibleMissingDependency
	{
		public ExportStrategyDependency Dependency { get; set; }

		public IExportStrategy Strategy { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplayString
		{
			get
			{
				if (Dependency.ImportType != null)
				{
					return Dependency.ImportType.FullName;
				}

				return Dependency.ImportName;
			}
		}
	}
}
