using System;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// This Lifestyle container will create an instance per scope instance.
	/// </summary>
	public sealed class SingletonPerScopeLifestyle : ILifestyle
	{
		private readonly string uniqueId = Guid.NewGuid().ToString();

		/// <summary>
		/// Dispose this Lifestyle container
		/// </summary>
		public void Dispose()
		{
			// do nothing because the object instances are disposed of by the scope that they are attached to
		}

		/// <summary>
		/// Objects managed by this container are transient. If true then the container will allow the export to be located in down facing scopes
		/// otherwise it will only be resolved in the current scope and in upward scopes (i.e. parent scope)
		/// </summary>
		public bool Transient
		{
			get { return true; }
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
			object returnValue = injectionContext.RequestingScope.GetExtraData(uniqueId);

			if (returnValue == null)
			{
				IInjectionScope requestScope = injectionContext.RequestingScope;

				returnValue = creationDelegate(exportStrategyScope, injectionContext);

				requestScope.SetExtraData(uniqueId, returnValue);

				IDisposable disposable = returnValue as IDisposable;

				if (disposable != null)
				{
					requestScope.AddDisposable(disposable);
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
			return new SingletonPerScopeLifestyle();
		}
	}
}