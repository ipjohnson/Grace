using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;
using Grace.Utilities;

namespace Grace.DependencyInjection.Lifestyle
{
	public class InScopeLifestyle : ILifestyle
	{
		private bool disposed;
		private readonly SafeDictionary<object, Tuple<object, IDisposalScope>> scopedObjects; 
		private readonly ExportActivationDelegate scopeDelegate;

		public InScopeLifestyle(ExportActivationDelegate scopeDelegate)
		{
			scopedObjects = new SafeDictionary<object, Tuple<object, IDisposalScope>>();
			this.scopeDelegate = scopeDelegate;
		}

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;

			foreach (KeyValuePair<object, Tuple<object, IDisposalScope>> scopedObject in scopedObjects)
			{
				scopedObject.Value.Item2.Dispose();
			}
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public bool Transient { get { return false; } }

		/// <summary>
		/// Locate object
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
			object scopeName = scopeDelegate(injectionScope, injectionContext);
			Tuple<object, IDisposalScope> trackedObject;

			if (!scopedObjects.TryGetValue(scopeName, out trackedObject))
			{
				IDisposalScope disposalScope = injectionContext.DisposalScope;
				IInjectionScope requestingScope = injectionContext.RequestingScope;

				IDisposalScope newDisposalScope = new DisposalScope();

				injectionContext.DisposalScope = newDisposalScope;
				injectionContext.RequestingScope = exportStrategy.OwningScope;

				object locatedObject = creationDelegate(injectionScope, injectionContext);

				injectionContext.DisposalScope = disposalScope;
				injectionContext.RequestingScope = requestingScope;

				if (locatedObject != null)
				{
					trackedObject = new Tuple<object, IDisposalScope>(locatedObject, newDisposalScope);

					scopedObjects[scopeName] = trackedObject;

					INotifyWhenDisposed notifyWhenDisposed = scopeName as INotifyWhenDisposed;

					if (notifyWhenDisposed != null)
					{
						notifyWhenDisposed.Disposed += (sender, args) =>
						                               {
							                               scopedObjects.Remove(scopeName);
																	 newDisposalScope.Dispose();
						                               };
					}

					return locatedObject;
				}
			}
			else
			{
				return trackedObject.Item1;
			}

			return null;
		}

		/// <summary>
		/// Clone lifestyle
		/// </summary>
		/// <returns></returns>
		public ILifestyle Clone()
		{
			return new InScopeLifestyle(scopeDelegate);
		}
	}
}
