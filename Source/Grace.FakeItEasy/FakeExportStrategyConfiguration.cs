using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.FakeItEasy
{
	public class FakeExportStrategyConfiguration<T> : IFakeExportStrategyConfiguration<T> where T : class
	{
		private readonly FakeExportStrategy<T> exportStrategy;

		public FakeExportStrategyConfiguration(FakeExportStrategy<T> exportStrategy)
		{
			this.exportStrategy = exportStrategy;
		}

		public IFakeExportStrategyConfiguration<T> WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> As<TExportType>()
		{
			exportStrategy.AddExportType(typeof(TExportType));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> As(Type exportType)
		{
			exportStrategy.AddExportType(exportType);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> AsName(string name)
		{
			exportStrategy.AddExportName(name);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> WhenInjectedInto<TInjected>()
		{
			exportStrategy.AddCondition(new WhenInjectedInto(typeof(TInjected)));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> WhenClassHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenClassHas(typeof(TAttr)));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> WhenMemberHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenMemberHas(typeof(TAttr)));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> WhenTargetHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenTargetHas(typeof(TAttr)));

			return this;
		}

		public IFakeExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> Arrange(Action<T> arrangeStatements)
		{
			exportStrategy.Arrange(arrangeStatements);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> Assert(Action<T> fakeAssert)
		{
			exportStrategy.Assert(fakeAssert);

			return this;
		}

		public IFakeExportStrategyConfiguration<T> AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		public IFakeExportStrategyConfiguration<T> LifestyleContainer(ILifestyle container)
		{
			exportStrategy.SetLifestyleContainer(container);

			return this;
		}
	}
}
