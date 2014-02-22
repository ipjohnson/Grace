using System;
using System.Web;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.MVC.Extensions;

namespace Grace.MVC.DependencyInjection
{
	public class WebSharedPerRequestLifestyle : ILifestyle
	{
		private readonly string uniqueId = Guid.NewGuid().ToString();

		public void Dispose()
		{
		}

		public bool Transient
		{
			get { return false; }
		}

		public object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope injectionScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy)
		{
			object returnValue = null;

			if (HttpContext.Current != null)
			{
				returnValue = HttpContext.Current.Items[uniqueId];
			}

			if (returnValue == null)
			{
				IDisposalScope disposalScope = injectionContext.DisposalScope;
				IInjectionScope requestingScope = injectionContext.RequestingScope;

				injectionContext.DisposalScope = MVCDisposalScopeProvider.GetDisposalScope();
				injectionContext.RequestingScope = exportStrategy.OwningScope;

				returnValue = creationDelegate(exportStrategy.OwningScope, injectionContext);

				injectionContext.DisposalScope = disposalScope;
				injectionContext.RequestingScope = requestingScope;

				if (returnValue != null && HttpContext.Current != null)
				{
					HttpContext.Current.Items[uniqueId] = returnValue;
				}
			}

			return returnValue;
		}

		public ILifestyle Clone()
		{
			return new WebSharedPerRequestLifestyle();
		}
	}
}