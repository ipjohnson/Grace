using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.WCF.LanguageExtensions;

namespace Grace.WCF.DependencyInjection
{
	public class WCFDisposalScopeProvider : IDisposalScopeProvider
	{
		public IDisposalScope ProvideDisposalScope(IInjectionScope injectionScope)
		{
			if (OperationContext.Current != null)
			{
				return OperationContext.Current.DisposalScope();
			}

			return injectionScope;
		}
	}
}
