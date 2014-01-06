using System;
using System.Threading;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// Thread static container will share an export per thread
	/// </summary>
	public sealed class ThreadStaticLifestyle : ILifestyle
	{
		private readonly ThreadLocal<object> instance = new ThreadLocal<object>();
		private IDisposalScope disposalScope = new DisposalScope();

		/// <summary>
		/// Dispose the container
		/// </summary>
		public void Dispose()
		{
			if (disposalScope != null)
			{
				instance.Dispose();

				disposalScope.Dispose();

				disposalScope = null;
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
			if (instance.Value != null)
			{
				return instance.Value;
			}

			IDisposalScope contextdisposalScope = injectionContext.DisposalScope;
			IInjectionScope requestingScope = injectionContext.RequestingScope;

			injectionContext.DisposalScope = exportStrategyScope;
			injectionContext.RequestingScope = exportStrategyScope;

			object instanceValue = creationDelegate(exportStrategyScope, injectionContext);

			injectionContext.DisposalScope = contextdisposalScope;
			injectionContext.RequestingScope = requestingScope;

			instance.Value = instanceValue;

			IDisposable disposable = instanceValue as IDisposable;

			if (disposable != null)
			{
				disposalScope.AddDisposable(disposable);
			}

			return instance.Value;
		}

		/// <summary>
		/// This method is used to clone a Lifestyle container
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new ThreadStaticLifestyle();
		}
	}
}