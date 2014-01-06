using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public class InjectionScopeImportService
	{
		public InjectionScopeImportService(IInjectionScope scope)
		{
			Scope = scope;
		}

		public IInjectionScope Scope { get; private set; }
	}
}
