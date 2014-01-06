using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;
using Moq;

namespace Grace.Moq
{
	/// <summary>
	/// Configuration object for Moq export strategy
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MoqExportStrategyConfiguration<T> : IMoqExportStrategyConfiguration<T> where T : class
	{
		private readonly MoqExportStrategy<T> exportStrategy;
		private readonly Type exportType;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="exportStrategy"></param>
		public MoqExportStrategyConfiguration(MoqExportStrategy<T> exportStrategy)
		{
			this.exportType = typeof(T);
			this.exportStrategy = exportStrategy;
		}

		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> As<TExportType>()
		{
			exportStrategy.AddExportType(typeof(TExportType));

			return this;
		}

		/// <summary>
		/// Export as a particular interface
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> As(Type exportType)
		{
			exportStrategy.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> AsName(string name)
		{
			exportStrategy.AddExportName(name);

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IMoqExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IMoqExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public IMoqExportStrategyConfiguration<T> AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> WhenInjectedInto<TInjected>()
		{
			exportStrategy.AddCondition(new WhenInjectedInto(typeof(TInjected)));

			return this;
		}

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> WhenClassHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenClassHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if the Property or method or constructor is attribute with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> WhenMemberHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenMemberHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> WhenTargetHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenTargetHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		/// <summary>
		/// Setup a moq for use
		/// </summary>
		/// <param name="mockSetup"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> Arrange(Action<Mock<T>> mockSetup)
		{
			exportStrategy.Arrange(mockSetup);

			return this;
		}

		/// <summary>
		/// Allows you to specify verify statements that will be executed when you cal Verify() on the container
		/// </summary>
		/// <param name="verifyMock"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> Assert(Action<Mock<T>> verifyMock)
		{
			exportStrategy.Assert(verifyMock);

			return this;
		}

		/// <summary>
		/// Make the moq a singleton
		/// </summary>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> AndSingleton()
		{
			exportStrategy.SetLifestyleContainer(new SingletonLifestyle());

			return this;
		}

		/// <summary>
		/// Setup a Lifestyle container for this Moq
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> LifestyleContainer(ILifestyle container)
		{
			exportStrategy.SetLifestyleContainer(container);

			return this;
		}

		/// <summary>
		/// Provide a list of strategies
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			yield return exportStrategy;
		}

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns></returns>
		public IMoqExportStrategyConfiguration<T> DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			return this;
		}
	}
}