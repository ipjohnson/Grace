using System.Diagnostics;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
	[DebuggerDisplay("{DebuggerValueDisplayString,nq}", Name = "{DebuggerNameDisplayString,nq}")]
	public class PossibleMissingDependency
	{
		public ExportStrategyDependency Dependency { get; set; }

		public IExportStrategy Strategy { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
      // ReSharper disable once UnusedMember.Local
		private string DebuggerNameDisplayString
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

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		// ReSharper disable once UnusedMember.Local
		private string DebuggerValueDisplayString
		{
			get { return "For " + Strategy.ActivationType.FullName; }
		}

	}
}