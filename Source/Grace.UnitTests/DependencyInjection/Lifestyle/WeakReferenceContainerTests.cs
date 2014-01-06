using System;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.LifestyleContainers
{
	public class WeakReferenceContainerTests
	{
		[Fact]
		public void DisposeTest()
		{
			WeakSingletonLifestyle container = new WeakSingletonLifestyle();

			container.Dispose();
		}

		[Fact]
		public void TransientTest()
		{
			WeakSingletonLifestyle container = new WeakSingletonLifestyle();

			Assert.False(container.Transient);
		}

		[Fact]
		public void CloneTest()
		{
			WeakSingletonLifestyle container = new WeakSingletonLifestyle();

			ILifestyle clone = container.Clone();

			Assert.NotNull(clone);
			Assert.IsType(typeof(WeakSingletonLifestyle), clone);
		}

		[Fact]
		public void ShareTest()
		{
			WeakSingletonLifestyle container = new WeakSingletonLifestyle();

			object basicService = container.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.NotNull(basicService);

			object basicService2 = container.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.True(ReferenceEquals(basicService, basicService2));
		}

		[Fact]
		public void WeakReferenceTest()
		{
			WeakSingletonLifestyle container = new WeakSingletonLifestyle();

			int count = GetBasicServiceCount(container, 1);

			GC.Collect();
			GC.WaitForPendingFinalizers();

			int count2 = GetBasicServiceCount(container, 2);

			Assert.Equal(2, count2);
		}

		private int GetBasicServiceCount(WeakSingletonLifestyle container, int count)
		{
			IBasicService basicService = (IBasicService)container.Locate(
				(x, y) => new BasicService { Count = count },
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.NotNull(basicService);

			return basicService.Count;
		}
	}
}