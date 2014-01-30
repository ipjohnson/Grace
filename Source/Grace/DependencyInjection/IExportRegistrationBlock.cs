using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// This interface allows you to register exports.
	/// </summary>
	public interface IExportRegistrationBlock
	{
		/// <summary>
		/// Scope this registration block is for
		/// </summary>
		[NotNull]
		IInjectionScope OwningScope { get; }

		/// <summary>
		/// Register an export by it's type. This is required when dealing with open generics
		/// </summary>
		/// <param name="type">type to export</param>
		/// <returns>configuration object</returns>
		[NotNull]
		IFluentExportStrategyConfiguration Export([NotNull]Type type);

		/// <summary>
		/// Export a class by it's type. This method allows you to specify things using linq expressions
		/// </summary>
		/// <typeparam name="T">type to export</typeparam>
		/// <returns>configuration object</returns>
		[NotNull]
		IFluentExportStrategyConfiguration<T> Export<T>();

		/// <summary>
		/// Export an enumeration of types all at one time
		/// </summary>
		/// <param name="types">collection of types to export</param>
		/// <returns>set configuration object</returns>
		[NotNull]
		IExportTypeSetConfiguration Export([NotNull]IEnumerable<Type> types);

		/// <summary>
		/// Register types from an assembly for exports.
		/// </summary>
		/// <param name="assembly">assembly to export</param>
		/// <returns>set configuration object</returns>
		[NotNull]
		IExportTypeSetConfiguration ExportAssembly([NotNull]Assembly assembly);

		/// <summary>
		/// Register types from a set of assemblies.
		/// </summary>
		/// <param name="assemblies">list of assemblies to export</param>
		/// <returns>set configuration object</returns>
		[NotNull]
		IExportTypeSetConfiguration ExportAssemblies([NotNull]IEnumerable<Assembly> assemblies);

		/// <summary>
		/// Export an instance of an object for a particular set of interfaces
		/// </summary>
		/// <typeparam name="T">instance type</typeparam>
		/// <param name="instance">instance to export</param>
		[NotNull]
		IFluentExportInstanceConfiguration<T> ExportInstance<T>([NotNull]T instance);

		/// <summary>
		/// Export an instance of an object for a particular set of interfaces
		/// </summary>
		/// <typeparam name="T">type to export</typeparam>
		/// <param name="instanceFunction">Func that creates instance</param>
		/// <returns>configuration object</returns>
		[NotNull]
		IFluentExportInstanceConfiguration<T> ExportInstance<T>([NotNull]ExportFunction<T> instanceFunction);

		/// <summary>
		/// Register an export function, it allows you to import properties, import method and activate methods
		/// of the T returned by the Func, if you don't need this functionality I recommend using ExportInstance
		/// </summary>
		/// <typeparam name="T">type to export</typeparam>
		/// <param name="exportFunction">Function to create instance of T</param>
		/// <returns>configuration object</returns>
		[NotNull]
		IFluentExportStrategyConfiguration<T> ExportFunc<T>([NotNull]ExportFunction<T> exportFunction);
		
		/// <summary>
		/// Simple export allows you to export a type with a smaller set of options.
		/// This method should be used to register types in child containers because the registration process is
		/// much faster than Export and ExportFunc (though the fastest registeration option is still ExportInstance)
		/// </summary>
		/// <typeparam name="T">type to export</typeparam>
		/// <returns>configuration object</returns>
		[NotNull]
		IFluentSimpleExportStrategyConfiguration SimpleExport<T>();

		/// <summary>
		/// Simple export allows you to export a type with a smaller set of options.
		/// This method should be used to register types in child containers because the registration process is
		/// much faster than Export and ExportFunc (though the fastest registeration option is still ExportInstance)
		/// </summary>
		/// <param name="type">export type</param>
		/// <returns>configuration object</returns>
		[NotNull]
		IFluentSimpleExportStrategyConfiguration SimpleExport([NotNull]Type type);

		/// <summary>
		/// Add an export strategy directly to a scope
		/// </summary>
		/// <param name="strategy">new startegy</param>
		void AddExportStrategy([NotNull]IExportStrategy strategy);

		/// <summary>
		/// Using this the developer can provide C# extensions that add to the registration block
		/// </summary>
		/// <param name="strategyProvider">new strategy provider</param>
		void AddExportProvider([NotNull] IExportStrategyProvider strategyProvider);
	}

}