using System;
using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.FauxClasses
{
	public class FauxDisposalScopeProvider : IDisposalScopeProvider
	{
		private readonly Func<IDisposalScope> scopeProvider;

		public FauxDisposalScopeProvider(Func<IDisposalScope> scopeProvider)
		{
			this.scopeProvider = scopeProvider;
		}

		public IDisposalScope ProvideDisposalScope(IInjectionScope injectionScope)
		{
			return scopeProvider();
		}
	}
}