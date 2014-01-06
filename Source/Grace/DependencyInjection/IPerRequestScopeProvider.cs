using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{

	public interface IPerRequestScopeProvider
	{
		IInjectionScope ProvideInjectionScope();
	}
}
