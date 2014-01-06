using System;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// The WeakSingletonContainer class will create an instance of the object and keep a weak reference to it
	/// A new instance will be requested upon request if the previous instance has been GC'd
	/// </summary>
	public sealed class WeakSingletonLifestyle : ILifestyle
	{
		private readonly object lockObject = new object();
		private WeakReference intanceRef;

		/// <summary>
		/// Dispose of container
		/// </summary>
		public void Dispose()
		{
			// do nothing because we don't track disposable objects
		}

		/// <summary>
		/// Objects managed by this container are transient. If true then the container will allow the export to be located in down facing scopes
		/// otherwise it will only be resolved in the current scope and in upward scopes (i.e. parent scope)
		/// </summary>
		public bool Transient
		{
			get { return false; }
		}

		/// <summary>
		/// This method is called by the export strategy when attempting to locate an export
		/// </summary>
		/// <param name="creationDelegate"></param>
		/// <param name="exportStrategyScope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope exportStrategyScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy)
		{
			object returnValue = null;

			if (intanceRef != null)
			{
				returnValue = intanceRef.Target;
			}

			if (returnValue == null)
			{
				lock (lockObject)
				{
					if (intanceRef != null)
					{
						returnValue = intanceRef.Target;
					}

					if (returnValue == null)
					{
						IDisposalScope disposalScope = injectionContext.DisposalScope;
						IInjectionScope requeInjectionScope = injectionContext.RequestingScope;

						// null scope because weak exports can't have 
						injectionContext.DisposalScope = null;
						injectionContext.RequestingScope = exportStrategyScope;

						returnValue = creationDelegate(exportStrategyScope, injectionContext);

						injectionContext.DisposalScope = disposalScope;
						injectionContext.RequestingScope = requeInjectionScope;

						if (returnValue != null)
						{
							intanceRef = new WeakReference(returnValue);
						}
					}
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
			return new WeakSingletonLifestyle();
		}
	}
}