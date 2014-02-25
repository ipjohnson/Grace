using System;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// Exports that use this will be shared per injection context and are considered transient
	/// </summary>
	public class SingletonPerInjectionContextLifestyle : ILifestyle
	{
		private readonly string key = Guid.NewGuid().ToString();

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Always true
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
			object returnValue = injectionContext.GetExtraData(key);

			if (returnValue == null)
			{
				returnValue = creationDelegate(injectionScope, injectionContext);

				if (returnValue != null)
				{
					injectionContext.SetExtraData(key, returnValue);
				}
			}

			return returnValue;
		}

		/// <summary>
		/// This method is used to clone a Lifestyle container
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new SingletonPerInjectionContextLifestyle();
		}
	}
}