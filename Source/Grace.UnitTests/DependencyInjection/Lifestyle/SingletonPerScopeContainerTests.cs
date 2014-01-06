using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.LifestyleContainers
{
	public class SingletonPerScopeContainerTests
	{
		[Fact]
		public void DisposeTest()
		{
			SingletonPerScopeLifestyle container = new SingletonPerScopeLifestyle();
			FauxInjectionScope fauxInjectionScope = new FauxInjectionScope();

			bool eventFired = false;

			object locatedObject =
				container.Locate((scope, context) =>
				                 {
					                 DisposableService disposableService = new DisposableService();

					                 disposableService.Disposing += (sender, args) => eventFired = true;

					                 return disposableService;
				                 },
					fauxInjectionScope,
					new FauxInjectionContext { RequestingScope = fauxInjectionScope },
					new FauxExportStrategy(() => new object()));

			Assert.NotNull(locatedObject);

			container.Dispose();

			Assert.False(eventFired);

			fauxInjectionScope.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void TransientTest()
		{
			SingletonPerScopeLifestyle container = new SingletonPerScopeLifestyle();

			Assert.True(container.Transient);
		}

		[Fact]
		public void CloneTest()
		{
			SingletonPerScopeLifestyle container = new SingletonPerScopeLifestyle();

			ILifestyle clone = container.Clone();

			Assert.NotNull(clone);
			Assert.IsType(typeof(SingletonPerScopeLifestyle), clone);
		}

		[Fact]
		public void SharedTest()
		{
			SingletonPerScopeLifestyle container = new SingletonPerScopeLifestyle();
			FauxInjectionScope requestingScope = new FauxInjectionScope();

			IBasicService basicService = (IBasicService)
				container.Locate((x, y) => new BasicService(),
					new FauxInjectionScope(),
					new FauxInjectionContext { RequestingScope = requestingScope },
					new FauxExportStrategy(() => 0));

			IBasicService testService = (IBasicService)
				container.Locate((x, y) => new BasicService(),
					new FauxInjectionScope(),
					new FauxInjectionContext { RequestingScope = requestingScope },
					new FauxExportStrategy(() => 0));

			Assert.True(ReferenceEquals(basicService, testService));
		}
	}
}