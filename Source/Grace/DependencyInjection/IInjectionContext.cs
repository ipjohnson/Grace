using System;
using System.Reflection;
using Grace.Data;
using Grace.Data.Immutable;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// There is one new Injection context object created for each injection request.
	/// It holds information pertinent to this injection
	/// Note: The implementation for IInjectionContext is not thread safe
	/// </summary>
	public interface IInjectionContext : IExtraDataContainer
	{
		/// <summary>
		/// Creates a clone of the injection context
		/// </summary>
		/// <returns></returns>
		IInjectionContext Clone();

		/// <summary>
		/// The disposal scope associated with this injection request
		/// </summary>
		[CanBeNull]
		IDisposalScope DisposalScope { get; set; }

		/// <summary>
		/// The scope that the request originated in
		/// </summary>
		[NotNull]
		IInjectionScope RequestingScope { get; set; }

		/// <summary>
		/// The target information for the current injection, 
		/// specifically what is the type you are being injected into and what is the PropertyInfo or ParameterInfo being injected into
		/// </summary>
		[CanBeNull]
		IInjectionTargetInfo TargetInfo { get; set; }

		/// <summary>
		/// When importing a property after construction this will contain the instance that is being injected
		/// </summary>
		[CanBeNull]
		object Instance { get; set; }

		/// <summary>
		/// Max resolve depth allowed
		/// </summary>
		int MaxResolveDepth { get; set; }

		/// <summary>
		/// Locate an export by type
		/// </summary>
		/// <returns></returns>
		object Locate<T>();

		/// <summary>
		/// Locate an export by type
		/// </summary>
		/// <returns></returns>
		object Locate([NotNull] Type type);

		/// <summary>
		/// Locate an export by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object Locate([NotNull] string name);

		/// <summary>
		/// Register an export by type for this injection context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exportFunction"></param>
		void Export<T>([NotNull] ExportFunction<T> exportFunction);

		/// <summary>
		/// Export a type with an activation delegate
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="activationDelegate"></param>
		void Export([NotNull] Type exportType, [NotNull] ExportActivationDelegate activationDelegate);

		/// <summary>
		/// Register an export by name for this injection context
		/// </summary>
		/// <param name="name"></param>
		/// <param name="activationDelegate"></param>
		void Export([NotNull] string name, [NotNull] ExportActivationDelegate activationDelegate);

		/// <summary>
		/// Push a current export strategy onto the stack
		/// </summary>
		/// <param name="exportStrategy">export strategy</param>
		void PushCurrentInjectionInfo<T>(IExportStrategy exportStrategy);

		/// <summary>
		/// Pop the current export strategy off the stack
		/// </summary>
		void PopCurrentInjectionInfo();

		/// <summary>
		/// Injection info all the way up the stack
		/// </summary>
		/// <returns></returns>
		ImmutableArray<CurrentInjectionInfo> GetInjectionStack();

	}
}