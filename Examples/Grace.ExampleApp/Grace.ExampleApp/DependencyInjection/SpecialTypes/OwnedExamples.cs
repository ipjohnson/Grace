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
	/// This example shows you how to import an Owned(T).
	/// All disposable object created during injection will be added to Owned(T).
	/// </summary>
	public class OwnedExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposedService>().As<IDisposedService>());

			Owned<IDisposedService> ownedFunc = container.Locate<Owned<IDisposedService>>();

			IDisposedService disposedService = ownedFunc.Value;

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
