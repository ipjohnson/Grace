using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.SpecialTypes
{
	/// <summary>
	/// This example shows how to import the disposal scope that the object was created in
	/// </summary>
	public class ImportDisposalScopeExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposalScopeImportService>());

			DisposalScopeImportService service = container.Locate<DisposalScopeImportService>();

			if (service == null)
			{
				throw new Exception("service cant be null");
			}

			if (service.Scope != container.RootScope)
			{
				throw new Exception("Disposal scope should be RootScope");
			}
		}
	}

	/// <summary>
	/// This example shows how to import the injection scope that the object is created in
	/// </summary>
	public class ImportInjectionScopeExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c=> c.Export<InjectionScopeImportService>());

			InjectionScopeImportService service = container.Locate<InjectionScopeImportService>();

			if (service == null)
			{
				throw new Exception("service should not be null");
			}

			if (service.Scope != container.RootScope)
			{
				throw new Exception("Scope should equal RootScope");
			}
		}
	}

	/// <summary>
	/// This example shows how to import the dependency injection container the object was created by 
	/// </summary>
	public class ImportIDependencyInjectionContainer : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DependencyInjectionContainerImportService>());

			DependencyInjectionContainerImportService service = container.Locate<DependencyInjectionContainerImportService>();

			if (service == null)
			{
				throw new Exception("service should not be null");
			}

			if (service.Container != container)
			{
				throw new Exception("container should be equal");
			}
		}
	}
}
