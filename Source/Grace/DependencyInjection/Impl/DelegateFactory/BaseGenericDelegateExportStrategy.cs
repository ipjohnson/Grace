using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.DelegateFactory
{
	public abstract class BaseGenericDelegateExportStrategy : IExportStrategy
	{
		public abstract object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider);

		public void Dispose()
		{
		}

		public abstract Type ActivationType { get; }

		public abstract string ActivationName { get; }

		public bool AllowingFiltering
		{
			get { return false; }
		}

		public IEnumerable<Attribute> Attributes
		{
			get { return new Attribute[0]; }
		}

		public IInjectionScope OwningScope { get; set; }

		public object Key { get; private set; }

		public IEnumerable<string> ExportNames
		{
			get
			{
				return new string[0]
					;
			}
		}

		public abstract IEnumerable<Type> ExportTypes { get; }

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
			get { return new ExportMetadata(null, new Dictionary<string, object>()); }
		}

		public virtual void Initialize()
		{
		}

		public bool MeetsCondition(IInjectionContext injectionContext)
		{
			return true;
		}

		public IEnumerable<IExportStrategy> SecondaryStrategies()
		{
			yield break;
		}

		public void EnrichWithDelegate(EnrichWithDelegate enrichWithDelegate)
		{
		}

		public IEnumerable<ExportStrategyDependency> DependsOn
		{
			get { yield break; }
		}
	}
}