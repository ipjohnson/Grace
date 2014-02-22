using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.NSubstitute
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISubstituteExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WithPriority(int priority);

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> As<TExportType>();

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment);

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> AsName(string name);

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the container
		/// </summary>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> AndSingleton();

		/// <summary>
		/// Export will be treated as a singleton for the lifetime of the scope
		/// </summary>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> AndSingletonPerScope();

		/// <summary>
		/// Exports will be trated as a singleton using a weak reference
		/// </summary>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> AndWeakSingleton();

		/// <summary>
		/// Attach a key to the export
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WithKey(object key);

		/// <summary>
		/// Specify a custom life cycle container for the export
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> UsingLifestyleContainer(ILifestyle container);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		ISubstituteExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		ISubstituteExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		ISubstituteExportStrategyConfiguration<T> AndCondition(IExportCondition condition);

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WhenInjectedInto<TInjected>();

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WhenClassHas<TAttr>();

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WhenMemberHas<TAttr>();

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WhenTargetHas<TAttr>();

		/// <summary>
		/// Sets up all public writable properties on the type to be injected
		/// </summary>
		/// <param name="required">are the properties required</param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> AutoWireProperties(bool required = false);

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue);

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">value for the parameter</param>
		/// <param name="paramName">name of the parameter, if null type matching is used</param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WithCtorParam<TParam>(TParam paramValue, string paramName = null);

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">Func(TParam) for the parameter</param>
		/// <param name="paramName">name of the parameter, if null type matching is used</param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WithCtorParam<TParam>(Func<TParam> paramValue, string paramName = null);

		/// <summary>
		/// Add a value to be used for constructor parameters
		/// </summary>
		/// <typeparam name="TParam">type of parameter</typeparam>
		/// <param name="paramValue">value provider for the parameter</param>
		/// <param name="paramName">name of the parameter, if null type matching is used</param>
		/// <param name="consider">filter which export to use</param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> WithCtorParam<TParam>(IExportValueProvider paramValue = null,
			string paramName = null,
			ExportStrategyFilter consider = null);

		/// <summary>
		/// You can provide a cleanup method to be called 
		/// </summary>
		/// <param name="disposalCleanupDelegate"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> DisposalCleanupDelegate(
			BeforeDisposalCleanupDelegate disposalCleanupDelegate);

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> EnrichWith(EnrichWithDelegate enrichWithDelegate);

		/// <summary>
		/// Allows you to specify a set of Substitutions to be invoked every time an object created
		/// </summary>
		/// <param name="setupAction"></param>
		/// <returns></returns>
		ISubstituteExportStrategyConfiguration<T> Arrange(Action<T> setupAction);
	}
}