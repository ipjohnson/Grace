using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.LifestyleContainers
{
	public class ThreadStaticContainerTests
	{
		[Fact]
		public void TransientTest()
		{
			ThreadStaticLifestyle container = new ThreadStaticLifestyle();

			Assert.False(container.Transient);
		}

		[Fact]
		public void CloneTest()
		{
			ThreadStaticLifestyle container = new ThreadStaticLifestyle();

			ILifestyle clone = container.Clone();

			Assert.NotNull(clone);
			Assert.IsType(typeof(ThreadStaticLifestyle), clone);
		}

		[Fact]
		public void ShareTest()
		{
			ThreadStaticLifestyle container = new ThreadStaticLifestyle();

			object instance = container.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.NotNull(instance);

			object instance2 = container.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.True(ReferenceEquals(instance, instance2));
		}

		[Fact]
		public void SeparatePerThread()
		{
			ThreadStaticLifestyle container = new ThreadStaticLifestyle();

			object instance = container.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.NotNull(instance);

			object instance2 = container.Locate((x, y) => new BasicService(),
				new FauxInjectionScope(),
				new FauxInjectionContext(),
				null);

			Assert.True(ReferenceEquals(instance, instance2));

			Task<object> newTask = Task.Factory.StartNew(
				() => container.Locate((x, y) => new BasicService(), new FauxInjectionScope(), new FauxInjectionContext(), null));

			newTask.Wait(1000);

			Assert.True(newTask.IsCompleted);
			Assert.False(ReferenceEquals(instance, newTask.Result));
		}

		[Fact]
		public void Dispose()
		{
			ThreadStaticLifestyle container = new ThreadStaticLifestyle();

			DisposableService instance =
				(DisposableService)
					container.Locate((x, y) => new DisposableService(), new FauxInjectionScope(), new FauxInjectionContext(), null);

			Assert.NotNull(instance);

			bool instanceDisposed = false;
			bool instance2Disposed = false;

			instance.Disposing += (sender, args) => instanceDisposed = true;

			Task<DisposableService> waitTask = Task.Factory.StartNew(() =>
				(DisposableService)
					container.Locate((x, y) => new DisposableService(),
						new FauxInjectionScope(),
						new FauxInjectionContext(),
						null));

			waitTask.Wait(1000);

			waitTask.Result.Disposing += (sender, args) => instance2Disposed = true;

			container.Dispose();

			Assert.True(instanceDisposed);
			Assert.True(instance2Disposed);
		}
	}
}