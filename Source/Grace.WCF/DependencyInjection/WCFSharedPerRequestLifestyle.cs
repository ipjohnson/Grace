using System;
using System.Collections.Generic;
using System.ServiceModel;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.WCF.LanguageExtensions;

namespace Grace.WCF.DependencyInjection
{
	public class WCFSharedPerRequestLifestyle : ILifestyle
	{
		private readonly string uniqueRequestKey = Guid.NewGuid().ToString();

		public void Dispose()
		{
		}

		public bool Transient
		{
			get { return true;}
		}

		public object Locate(ExportActivationDelegate creationDelegate,
			IInjectionScope injectionScope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy)
		{
			IDictionary<object, object> items = OperationContext.Current.Items();
			object returnValue = items[uniqueRequestKey];

			if (returnValue == null)
			{
				IDisposalScope disposalScope = injectionContext.DisposalScope;
				IInjectionScope requestingScope = injectionContext.RequestingScope;

				injectionContext.DisposalScope = OperationContext.Current.DisposalScope();
				injectionContext.RequestingScope = exportStrategy.OwningScope;

				returnValue = creationDelegate(exportStrategy.OwningScope, injectionContext);

				injectionContext.DisposalScope = disposalScope;
				injectionContext.RequestingScope = requestingScope;

				if (returnValue != null)
				{
					items[uniqueRequestKey] = returnValue;
				}
			}

			return returnValue;
		}

		public ILifestyle Clone()
		{
			return new WCFSharedPerRequestLifestyle();
		}
	}
}