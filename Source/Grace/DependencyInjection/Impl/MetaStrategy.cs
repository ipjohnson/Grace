using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	public class MetaStrategy<T> : IExportStrategy
	{
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider)
		{
			IExportStrategy strategy = FindExportStrategy(exportInjectionScope, context, consider);

			if (strategy != null)
			{
				T activatedObject = (T)strategy.Activate(exportInjectionScope, context, consider);

				return new Meta<T>(activatedObject, strategy.Metadata);
			}

			return null;
		}

		private IExportStrategy FindExportStrategy(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			IEnumerable<IExportStrategy> strategies = exportInjectionScope.GetStrategies(typeof(T), context, consider);

			IExportStrategy strategy = strategies.FirstOrDefault();

			if (strategy == null && exportInjectionScope.ParentScope != null)
			{
				return FindExportStrategy(exportInjectionScope.ParentScope, context, consider);
			}

			return strategy;
		}

		public void Dispose()
		{

		}

		public Type ActivationType
		{
			get { return typeof(Meta<T>); }
		}

		public string ActivationName
		{
			get { return typeof(Meta<T>).FullName; }
		}

		public bool AllowingFiltering
		{
			get { return false; }
		}

		public IEnumerable<Attribute> Attributes
		{
			get { return new Attribute[0]; }
		}

		public IInjectionScope OwningScope { get; set; }

		public object Key
		{
			get { return null; }
		}

		public IEnumerable<string> ExportNames
		{
			get { yield return typeof(Meta<T>).FullName; }
		}

		public ExportEnvironment Environment
		{
			get { return ExportEnvironment.Any; }
		}

		public int Priority
		{
			get { return 0; }
		}

		public ILifestyle Lifestyle
		{
			get { return null; }
		}

		public bool HasConditions
		{
			get { return false; }
		}

		public bool ExternallyOwned
		{
			get { return false; }
		}

		public IExportMetadata Metadata
		{
			get { return new ExportMetadata(null,new Dictionary<string, object>()); }
		}

		public void Initialize()
		{

		}

		public bool MeetsCondition(IInjectionContext injectionContext)
		{
			return true;
		}

		public IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			return new IExportStrategy[0];
		}

		public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{

		}

		public IEnumerable<ExportStrategyDependency> DependsOn { get; private set; }
	}
}
