using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;
using Moq;

namespace Grace.Moq
{
	/// <summary>
	/// Configure a Moq export strategy for use in a container or scope
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMoqExportStrategyConfiguration<T> where T : class
	{
		/// <summary>
		/// Export the type with the specified priority
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> WithPriority(int priority);

		/// <summary>
		/// Export as a specific type (usually an interface)
		/// </summary>
		/// <typeparam name="TExportType"></typeparam>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> As<TExportType>();

		/// <summary>
		/// Export as a particular interface
		/// </summary>
		/// <param name="exportType"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> As(Type exportType);

		/// <summary>
		/// Export type in this Environment (ExportEnvironement is a flag so it can be or'd)
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> InEnvironment(ExportEnvironment environment);

		/// <summary>
		/// Export the type under the specified name
		/// </summary>
		/// <param name="name">name to export under</param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> AsName(string name);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		IMoqExportStrategyConfiguration<T> When(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		IMoqExportStrategyConfiguration<T> Unless(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		IMoqExportStrategyConfiguration<T> AndCondition(IExportCondition condition);

		/// <summary>
		/// Applies a new WhenInjectedInto condition on the export, using the export only when injecting into the specified class
		/// </summary>
		/// <typeparam name="TInjected"></typeparam>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> WhenInjectedInto<TInjected>();

		/// <summary>
		/// Applies a WhenClassHas condition, using the export only if injecting into a class that is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> WhenClassHas<TAttr>();

		/// <summary>
		/// Applies a WhenMemberHas condition, using the export only if the Property or method or constructor is attribute with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> WhenMemberHas<TAttr>();

		/// <summary>
		/// Applies a WhenTargetHas condition, using the export only if the Property or Parameter is attributed with TAttr
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> WhenTargetHas<TAttr>();

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName"></param>
		/// <param name="metadataValue"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> WithMetadata(string metadataName, object metadataValue);

		/// <summary>
		/// Setup a moq for use
		/// </summary>
		/// <param name="mockSetup"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> Arrange(Action<Mock<T>> mockSetup);

		/// <summary>
		/// Allows you to specify verify statements that will be executed when you cal Verify() on the container
		/// </summary>
		/// <param name="verifyMock"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> Assert(Action<Mock<T>> verifyMock);

		/// <summary>
		/// Make the moq a singleton
		/// </summary>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> AndSingleton();

		/// <summary>
		/// Setup a life cycle contianer for this Moq
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		IMoqExportStrategyConfiguration<T> LifestyleContainer(ILifestyle container);
	}
}