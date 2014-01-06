using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.WCF.LanguageExtensions;

namespace Grace.WCF.DependencyInjection
{
	public class WCFSharedPerRequestLifestyle : ILifestyle
	{
		private string uniqueRequestKey = Guid.NewGuid().ToString();

		public void Dispose()
		{
			
		}

		public bool Transient { get; private set; }

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
