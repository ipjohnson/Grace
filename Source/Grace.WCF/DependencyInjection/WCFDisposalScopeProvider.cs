using System.ServiceModel;
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