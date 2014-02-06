using System;
using Grace.Logging;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// Exports that use this will be shared per request
	/// </summary>
	public class SingletonPerRequestLifestyle : ILifestyle
	{
		private ILifestyle LifestyleContainer;

		/// <summary>
		/// Dispose the container
		/// </summary>
		public void Dispose()
		{
			if (LifestyleContainer != null)
			{
				LifestyleContainer.Dispose();
			}
		}

		/// <summary>
		/// Singleton Per Request are considered transient when resolving
		/// </summary>
		public bool Transient
		{
			get { return true; }
		}

		/// <summary>
		/// This method is called by the export strategy when attempting to locate an export
		/// </summary>
		/// <param name="creationDelegate"></param>
		/// <param name="injectionScope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope injectionScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy)
		{
			if (LifestyleContainer == null)
			{
				LifestyleContainer = LocateContainer(exportStrategy.OwningScope, injectionContext);
			}

			return LifestyleContainer.Locate(creationDelegate, injectionScope, injectionContext, exportStrategy);
		}

		/// <summary>
		/// This method is used to clone a Lifestyle container
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new SingletonPerRequestLifestyle();
		}

		/// <summary>
		/// Locates an ILifestyle
		/// </summary>
		/// <param name="injectionScope"></param>
		/// <param name="injectionContext"></param>
		/// <returns></returns>
		private static ILifestyle LocateContainer(IInjectionScope injectionScope, IInjectionContext injectionContext)
		{
			ILifestyle returnValue = null;

			try
			{
				IPerRequestLifestyleProvider provider =
					injectionScope.Locate<IPerRequestLifestyleProvider>(injectionContext);

				if (provider != null)
				{
					returnValue = provider.ProvideContainer();
				}
			}
			catch (Exception exp)
			{
				Logger.Error("Exception throw while trying to locate IPerRequestLifestyleProvider", "SingletonPerRequest", exp);
			}

			return returnValue ?? new SingletonPerInjectionContextLifestyle();
		}
	}
}