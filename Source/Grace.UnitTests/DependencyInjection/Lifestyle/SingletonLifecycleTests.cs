using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.LifestyleContainers
{
	public class SingletonLifestyleTests
	{
		[Fact]
		public void TransientTest()
		{
			SingletonLifestyle lifestyle = new SingletonLifestyle();

			Assert.False(lifestyle.Transient);
		}

		[Fact]
		public void CloneTest()
		{
			SingletonLifestyle lifestyle = new SingletonLifestyle();

			ILifestyle clone = lifestyle.Clone();

			Assert.NotNull(clone);
			Assert.IsType(typeof(SingletonLifestyle), clone);
		}

		[Fact]
		public void ShareTest()
		{
			SingletonLifestyle lifestyle = new SingletonLifestyle();

			object instance = lifestyle.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				new FauxExportStrategy(() => new object()));

			Assert.NotNull(instance);

			object instance2 = lifestyle.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				new FauxExportStrategy(() => new object()));

			Assert.True(ReferenceEquals(instance, instance2));
		}

		[Fact]
		public void DisposeTest()
		{
			SingletonLifestyle lifestyle = new SingletonLifestyle();

			IDisposableService disposableService =
				(IDisposableService)
					lifestyle.Locate((x, y) => new DisposableService(),
						new FauxInjectionScope(),
						new FauxInjectionContext(),
						new FauxExportStrategy(() => new object()));

			bool eventCalled = false;

			disposableService.Disposing += (sender, args) => eventCalled = true;

			lifestyle.Dispose();

			Assert.True(eventCalled);
		}
	}
}