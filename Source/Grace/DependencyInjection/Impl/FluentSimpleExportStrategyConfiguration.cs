using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	public class FluentSimpleExportStrategyConfiguration : IFluentSimpleExportStrategyConfiguration, IExportStrategyProvider
	{
		private ConfigurableExportStrategy exportStrategy;

		public FluentSimpleExportStrategyConfiguration(ConfigurableExportStrategy exportStrategy)
		{
			this.exportStrategy = exportStrategy;
		}

		public IFluentSimpleExportStrategyConfiguration WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration WithKey(object key)
		{
			exportStrategy.SetKey(key);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration As(Type exportType)
		{
			exportStrategy.AddExportType(exportType);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration As<T>()
		{
			exportStrategy.AddExportType(typeof(T));

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration AsName(string name)
		{
			exportStrategy.AddExportName(name);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration AndSingletonPerScope()
		{
			exportStrategy.SetLifestyleContainer(new SingletonPerScopeLifestyle());

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration AndWeakSingleton()
		{
			exportStrategy.SetLifestyleContainer(new WeakSingletonLifestyle());

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration ExternallyOwned()
		{
			exportStrategy.SetExternallyOwned();

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration UsingLifestyle(ILifestyle lifestyle)
		{
			exportStrategy.SetLifestyleContainer(lifestyle);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration When(ExportConditionDelegate conditionDelegate)
		{
			throw new NotImplementedException();
		}

		public IFluentSimpleExportStrategyConfiguration Unless(ExportConditionDelegate conditionDelegate)
		{
			throw new NotImplementedException();
		}

		public IFluentSimpleExportStrategyConfiguration AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName,metadataValue);

			return this;
		}

		public IFluentSimpleExportStrategyConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate)
		{
			exportStrategy.EnrichWithDelegate(enrichWithDelegate);

			return this;
		}

		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			yield return exportStrategy;
		}
	}
}
