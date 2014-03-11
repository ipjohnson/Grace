using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;

namespace Grace.ExampleApp.DependencyInjection
{
	public class DependencyInjectionExampleModule : IExampleModule
	{
		private readonly IEnumerable<IExampleSubModule<DependencyInjectionExampleModule>> subModules;

		public DependencyInjectionExampleModule(IEnumerable<IExampleSubModule<DependencyInjectionExampleModule>> subModules)
		{
			this.subModules = subModules;
		}

		public string ModuleName
		{
			get { return typeof(DependencyInjectionExampleModule).Name; }
		}

		public void Execute()
		{
			foreach (IExampleSubModule<DependencyInjectionExampleModule> exampleSubModule in subModules)
			{
				exampleSubModule.Execute();
			}
		}
	}
}
