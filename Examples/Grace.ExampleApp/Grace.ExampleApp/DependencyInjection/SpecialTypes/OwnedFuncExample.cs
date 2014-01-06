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
	/// This example show how you can import an Owned(Func(T)).
	/// All disposable object created by the Func will be added to Owned object for disposable tracking
	/// </summary>
	public class OwnedFuncExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposedService>().As<IDisposedService>());

			Owned<Func<IDisposedService>> ownedFunc = container.Locate<Owned<Func<IDisposedService>>>();

			IDisposedService disposedService = ownedFunc.Value();

			bool disposedCalled = false;

			disposedService.Disposed += (sender, args) => disposedCalled = true;

			ownedFunc.Dispose();
			
			if (!disposedCalled)
			{
				throw new Exception("Should have disposed service");
			}
		}
	}
}
