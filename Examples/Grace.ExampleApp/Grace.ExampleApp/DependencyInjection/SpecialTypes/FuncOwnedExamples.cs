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
	/// This example shows you have to import a Func(Owned(T)) that creates Owned(T) that represent a unit of work
	/// </summary>
	public class FuncOwnedExample : IExample<SpecialTypesSubModule>
	{
		public void ExecuteExample()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposedService>().As<IDisposedService>());

			Func<Owned<IDisposedService>> ownedFunc = container.Locate<Func<Owned<IDisposedService>>>();

			Owned<IDisposedService> owned1 = ownedFunc();
			Owned<IDisposedService> owned2 = ownedFunc();

			bool owned1Disposed = false;
			bool owned2Disposed = false;

			owned1.Value.Disposed += (sender, args) => owned1Disposed = true;

			owned2.Value.Disposed += (sender, args) => owned2Disposed = true;

			owned1.Dispose();

			if (!owned1Disposed)
			{
				throw new Exception("owned1 should have been disposed");
			}

			if (owned2Disposed)
			{
				throw new Exception("owned2 should not be disposed yet");
			}

			owned1Disposed = false;

			owned2.Dispose();

			if (!owned2Disposed)
			{
				throw new Exception("owned2 should have been disposed");
			}

			if (owned1Disposed)
			{
				throw new Exception("owned1Diposed should be false");
			}
		}
	}
}
