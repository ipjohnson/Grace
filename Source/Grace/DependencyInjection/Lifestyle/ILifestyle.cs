using System;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// ILifestyle objects are used to manage the Lifestyle of a particular export
	/// Singleton, Shared, Cached, and PerRequest are examples of Lifestyle containers
	/// </summary>
	public interface ILifestyle : IDisposable
	{
		/// <summary>
		/// If true then the container will allow the dependencies to be located in down facing scopes
		/// otherwise they will only be resolved in the current scope and in upward scopes (i.e. parent scope)
		/// </summary>
		bool Transient { get; }

		/// <summary>
		/// This method is called by the export strategy when attempting to locate an export
		/// </summary>
		/// <param name="creationDelegate"></param>
		/// <param name="injectionScope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope injectionScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy);

		/// <summary>
		/// This method is used to clone a Lifestyle container
		/// </summary>
		/// <returns></returns>
		ILifestyle Clone();
	}
}