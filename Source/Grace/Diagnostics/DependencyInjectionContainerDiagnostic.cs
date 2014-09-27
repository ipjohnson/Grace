using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grace.Data;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Runs diagnostics on a dependency injection container
    /// </summary>
	public class DependencyInjectionContainerDiagnostic
	{
		private bool initialized;
		private readonly IDependencyInjectionContainer container;
		private IEnumerable<PossibleMissingDependency> possibleMissingDependencies;

        /// <summary>
        /// Default constructor takes a DI container
        /// </summary>
        /// <param name="container">container to diagnose</param>
		public DependencyInjectionContainerDiagnostic(IDependencyInjectionContainer container)
		{
			this.container = container;
		}

        /// <summary>
        /// Environment for the container
        /// </summary>
		public ExportEnvironment Environment
		{
			get { return container.Environment; }
		}

        /// <summary>
        /// Auto register unknown concrete types
        /// </summary>
		public bool AutoRegisterUnknown
		{
			get { return container.AutoRegisterUnknown; }
		}

        /// <summary>
        /// RootScope for the container
        /// </summary>
		public IInjectionScope RootScope
		{
			get { return container.RootScope; }
		}

        /// <summary>
        /// Exports for the container
        /// </summary>
		public IEnumerable<IExportStrategy> Exports
		{
			get { return container.GetAllStrategies(); }
		}

        /// <summary>
        /// List of possible missing dependencies.
        /// Note: This is just a possible missing dependency
        /// Using static analysis this is a best attempt at resolving.
        /// Because of conditions and other factors it's possible to have no missing dependencies listed
        /// and still fail to resolve and vice versa
        /// </summary>
		[DebuggerDisplay("{DebuggerPossibleMissingDependenciesString,nq}", Name = "Possible Missing Dependencies")]
		public IEnumerable<PossibleMissingDependency> PossibleMissingDependencies
		{
			get
			{
				Initialize();

				return possibleMissingDependencies;
			}
		}

        /// <summary>
        /// All exports by name
        /// </summary>
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

				sortList.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.CurrentCulture));

				return new List<ExportListDebuggerView>(sortList.Select(x => x.Value));
			}
		}

        /// <summary>
        /// All export by type
        /// </summary>
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
							view = new ExportListDebuggerView(ReflectionService.GetFriendlyNameForType(exportType));

							returnValue[exportType] = view;
						}

						view.Add(exportStrategy);
					}
				}

				List<KeyValuePair<Type, ExportListDebuggerView>> sortList =
					new List<KeyValuePair<Type, ExportListDebuggerView>>(returnValue);

				sortList.Sort((x, y) => string.CompareOrdinal(x.Key.FullName, y.Key.FullName));

				return new List<ExportListDebuggerView>(sortList.Select(x => x.Value));
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		// ReSharper disable once UnusedMember.Local
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
				InjectionScopeDiagnostic.CalculatePossibleMissingDependencies(container.RootScope);
		}
	}
}