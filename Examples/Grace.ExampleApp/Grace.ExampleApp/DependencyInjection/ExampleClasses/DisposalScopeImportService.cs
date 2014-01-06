using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public class DisposalScopeImportService
	{
		public DisposalScopeImportService(IDisposalScope scope)
		{
			Scope = scope;
		}

		public IDisposalScope Scope { get; private set; }
	}
}
