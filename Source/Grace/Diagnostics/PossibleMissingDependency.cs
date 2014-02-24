using System.Diagnostics;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
	[DebuggerDisplay("{DebuggerDisplayString,nq}", Name = "Missing Dependency")]
	public class PossibleMissingDependency
	{
		public ExportStrategyDependency Dependency { get; set; }

		public IExportStrategy Strategy { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
      // ReSharper disable once UnusedMember.Local
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