using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.FakeItEasy
{
	/// <summary>
	/// Configures a Fake object for export
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFakeExportStrategyConfiguration<out T> where T : class
	{
		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> WithPriority(int priority);

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> As<TExportType>();

		/// <summary>
		/// Export as a particular interface
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> As(Type exportType);

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment);

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> AsName(string name);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		IFakeExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		IFakeExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		IFakeExportStrategyConfiguration<T> AndCondition(IExportCondition condition);

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> WhenInjectedInto<TInjected>();

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> WhenClassHas<TAttr>();

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if the Property or method or constructor is attribute with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> WhenMemberHas<TAttr>();

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> WhenTargetHas<TAttr>();

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue);

		/// <summary>
		/// Setup a moq for use
		/// </summary>
		/// <param name="arrangeStatements"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> Arrange(Action<T> arrangeStatements);

		/// <summary>
		/// Allows you to specify verify statements that will be executed when you cal Verify() on the container
		/// </summary>
		/// <param name="assertStatements"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> Assert(Action<T> assertStatements);

		/// <summary>
		/// Make the moq a singleton
		/// </summary>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> AndSingleton();

		/// <summary>
		/// Setup a life cycle contianer for this Moq
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		IFakeExportStrategyConfiguration<T> LifestyleContainer(ILifestyle container);
	}
}