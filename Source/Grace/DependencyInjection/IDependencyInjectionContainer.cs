using System;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// This interface defines the base functionality for the container
	/// </summary>
	public interface IDependencyInjectionContainer : IExportLocator, IDisposalScope
	{
		/// <summary>
		/// If a concrete type is requested and it is not registered an export strategy will be created.
		/// Note: It will be scanned for attributes
		/// </summary>
		bool AutoRegisterUnknown { get; set; }

		/// <summary>
		/// If true exception will be thrown if a type can't be located, otherwise it will be caught and errors logged
		/// False by default
		/// </summary>
		bool ThrowExceptions { get; set; }

		/// <summary>
		/// The root scope for the container
		/// </summary>
        [NotNull]
		IInjectionScope RootScope { get; }

		/// <summary>
		/// Handling this event allows you to add exports to the container when an export can't be located.
		/// </summary>
		event EventHandler<ResolveUnknownExportArgs> ResolveUnknownExports;

		/// <summary>
		/// Black lists a particular export (Fullname)
		/// </summary>
		/// <param name="exportType">full name of type to black list</param>
		void BlackListExport([NotNull] string exportType);

		/// <summary>
		/// Black list a particular export by Type
		/// </summary>
		/// <param name="exportType">type to black list</param>
		void BlackListExportType([NotNull] Type exportType);

		/// <summary>
		/// This method can be used to configure a particular scope in the container
		/// </summary>
		/// <param name="registrationDelegate">delegate to be used to configure the scope</param>
		/// <param name="scopeName">scope name to configure</param>
		void Configure([NotNull] string scopeName, [NotNull] ExportRegistrationDelegate registrationDelegate);

		/// <summary>
		/// This method can be used to configure a particular scope in the container
		/// </summary>
		/// <param name="configurationModule">configuration module object to be used configure the scope</param>
		/// <param name="scopeName">name of scope to configure</param>
		void Configure([NotNull] string scopeName, [NotNull] IConfigurationModule configurationModule);

        /// <summary>
        /// Add IStrategyInspector to the container. It will be called 
        /// </summary>
        /// <param name="inspector"></param>
        void AddInspector([NotNull]IStrategyInspector inspector);
	}
}