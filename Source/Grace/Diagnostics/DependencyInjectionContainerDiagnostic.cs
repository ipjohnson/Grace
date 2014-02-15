using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
	public class DependencyInjectionContainerDiagnostic
	{
		private bool initialized;
		private IDependencyInjectionContainer container;
		private IEnumerable<PossibleMissingDependency> possibleMissingDependencies; 

		public DependencyInjectionContainerDiagnostic(IDependencyInjectionContainer container)
		{
			this.container = container;
		}

		public ExportEnvironment Environment
		{
			get { return container.Environment; }
		}

		public bool AutoRegisterUnknown
		{
			get { return container.AutoRegisterUnknown; }
		}

		public IInjectionScope RootScope
		{
			get { return container.RootScope; }
		}

		public IEnumerable<IExportStrategy> Exports
		{
			get { return container.GetAllStrategies(); }
		}

		[DebuggerDisplay("{DebuggerPossibleMissingDependenciesString,nq}", Name = "Possible Missing Dependencies")]
		public IEnumerable<PossibleMissingDependency> PossibleMissingDependencies
		{
			get
			{
				Initialize();

				return possibleMissingDependencies;
			}
		}

		public IEnumerable<ExportListDebuggerView> ExportsByName
		{
			get
			{
				Dictionary<string, ExportListDebuggerView> returnValue = new Dictionary<string, ExportListDebuggerView>();

				foreach (IExportStrategy exportStrategy in container.GetAllStrategies())
				{
					foreach (string exportName in exportStrategy.ExportNames)
					{
						ExportListDebuggerView view;

						if (!returnValue.TryGetValue(exportName, out view))
						{
							view = new ExportListDebuggerView(exportName);

							returnValue[exportName] = view;
						}

						view.Add(exportStrategy);
					}
				}

				List<KeyValuePair<string, ExportListDebuggerView>> sortList =
					new List<KeyValuePair<string, ExportListDebuggerView>>(returnValue);

				sortList.Sort((x, y) => string.Compare(x.Key, y.Key));

				return new List<ExportListDebuggerView>(sortList.Select(x => x.Value));
			}
		}

		public IEnumerable<ExportListDebuggerView> ExportsByType
		{
			get
			{
				Dictionary<Type, ExportListDebuggerView> returnValue = new Dictionary<Type, ExportListDebuggerView>();

				foreach (IExportStrategy exportStrategy in container.GetAllStrategies())
				{
					foreach (Type exportType in exportStrategy.ExportTypes)
					{
						ExportListDebuggerView view;

						if (!returnValue.TryGetValue(exportType, out view))
						{
							view = new ExportListDebuggerView(exportType.FullName);

							returnValue[exportType] = view;
						}

						view.Add(exportStrategy);
					}
				}

				List<KeyValuePair<Type, ExportListDebuggerView>> sortList =
					new List<KeyValuePair<Type, ExportListDebuggerView>>(returnValue);

				return new List<ExportListDebuggerView>(sortList.Select(x => x.Value));
			}
		}


		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerPossibleMissingDependenciesString
		{
			get { return "Count = " + PossibleMissingDependencies.Count(); }
		}

		private void Initialize()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;

			possibleMissingDependencies = 
				IInjectionScopeDiagnostic.CalculatePossibleMissingDependencies(container.RootScope);
		}
	}
}
