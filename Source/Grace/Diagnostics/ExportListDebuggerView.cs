using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grace.Data;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
	[DebuggerDisplay("{DisplayValue,nq}", Name = "{NameDisplayValue,nq}")]
	public class ExportListDebuggerView
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<IExportStrategy> strategies;

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
        // ReSharper disable once UnusedMember.Local
		private string DisplayValue
		{
			get
			{
				return "Count = " + Strategies.Count();
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        // ReSharper disable once UnusedMember.Local
		private string NameDisplayValue
		{
			get { return name + " "; }
		}
	}
}