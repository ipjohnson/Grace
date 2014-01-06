using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Configures an instance for export
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ExportInstanceConfiguration<T> : IFluentExportInstanceConfiguration<T>, IExportStrategyProvider
	{
		private readonly IConfigurableExportStrategy exportStrategy;
		private readonly Type exportType;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="exportStrategy"></param>
		public ExportInstanceConfiguration(Type exportType, IConfigurableExportStrategy exportStrategy)
		{
			this.exportType = exportType;
			this.exportStrategy = exportStrategy;
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
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> WithPriority(int priority)
		{
			exportStrategy.SetPriority(priority);

			return this;
		}

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> As<TExportType>()
		{
			exportStrategy.AddExportType(typeof(TExportType));

			return this;
		}

		/// <summary>
		/// Export as a particular interface
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> As(Type exportType)
		{
			exportStrategy.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> InEnvironment(ExportEnvironment environment)
		{
			exportStrategy.SetEnvironment(environment);

			return this;
		}

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> AsName(string name)
		{
			exportStrategy.AddExportName(name.ToLowerInvariant());

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportInstanceConfiguration<T> When(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new WhenCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		public IFluentExportInstanceConfiguration<T> Unless(ExportConditionDelegate conditionDelegate)
		{
			exportStrategy.AddCondition(new UnlessCondition(conditionDelegate));

			return this;
		}

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		public IFluentExportInstanceConfiguration<T> AndCondition(IExportCondition condition)
		{
			exportStrategy.AddCondition(condition);

			return this;
		}

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> WhenInjectedInto<TInjected>()
		{
			exportStrategy.AddCondition(new WhenInjectedInto(typeof(TInjected)));

			return this;
		}

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> WhenClassHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenClassHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if the Property or method or constructor is attribute with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> WhenMemberHas<TAttr>()
		{
			exportStrategy.AddCondition(new WhenMemberHas(typeof(TAttr)));

			return this;
		}

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public IFluentExportInstanceConfiguration<T> WhenTargetHas<TAttr>()
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
		public IFluentExportInstanceConfiguration<T> WithMetadata(string metadataName, object metadataValue)
		{
			exportStrategy.AddMetadata(metadataName, metadataValue);

			return this;
		}

		public IFluentExportInstanceConfiguration<T> DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate)
		{
			return this;
		}

		public IFluentExportInstanceConfiguration<T> UsingLifestyle(ILifestyle lifestyle)
		{
			exportStrategy.SetLifestyleContainer(lifestyle);

			return this;
		}
	}
}