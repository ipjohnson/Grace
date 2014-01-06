using System;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// The SingletonContainer is a Lifestyle container that when used on an export makes it into a singleton
	/// </summary>
	public sealed class SingletonLifestyle : ILifestyle
	{
		private readonly object lockObject = new object();
		private bool disposed;
		private volatile object instance;

		/// <summary>
		/// Dispose of the container
		/// </summary>
		public void Dispose()
		{
			if (!disposed)
			{
				IDisposable disposable = instance as IDisposable;

				if (disposable != null)
				{
					disposable.Dispose();

					instance = null;
				}

				disposed = true;
			}
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
			if (instance == null)
			{
				lock (lockObject)
				{
					if (instance == null)
					{
						IDisposalScope disposalScope = injectionContext.DisposalScope;
						IInjectionScope requestingScope = injectionContext.RequestingScope;

						injectionContext.DisposalScope = exportStrategy.OwningScope;
						injectionContext.RequestingScope = exportStrategy.OwningScope;

						instance = creationDelegate(exportStrategyScope, injectionContext);

						injectionContext.DisposalScope = disposalScope;
						injectionContext.RequestingScope = requestingScope;
					}
				}
			}

			return instance;
		}

		/// <summary>
		/// This method is used to clone a Lifestyle container
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new SingletonLifestyle();
		}
	}
}