using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class LazyTests
	{
		[Fact]
		public void LocateLazy()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<LazyService>().As<ILazyService>());

			LazyService.Created = false;

			var lazy = container.Locate<Lazy<ILazyService>>();

			Assert.NotNull(lazy);
			Assert.False(LazyService.Created);

			ILazyService service = lazy.Value;

			Assert.NotNull(service);
			Assert.True(LazyService.Created);
		}

		[Fact]
		public void LocateIEnumerableLazy()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<LazyService>().As<ILazyService>());

			LazyService.Created = false;

			var lazies = container.LocateAll<Lazy<ILazyService>>();

			Assert.NotNull(lazies);
			Assert.False(LazyService.Created);

			ILazyService service = lazies.First().Value;

			Assert.NotNull(service);
			Assert.True(LazyService.Created);

		}

		[Fact]
		public void CombinedSpecialTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<LazyService>().As<ILazyService>().WithMetadata("Hello", "World"));

			LazyService.Created = false;

			var lazy = container.Locate<Lazy<Meta<ILazyService>>>();

			Assert.NotNull(lazy);
			Assert.False(LazyService.Created);

			Assert.NotNull(lazy.Value);
			Assert.NotNull(lazy.Value.Value);

			ILazyService service = lazy.Value.Value;

			Assert.NotNull(service);
			Assert.True(LazyService.Created);

			var metadata = lazy.Value.Metadata.First();

			Assert.Equal("Hello", metadata.Key);
			Assert.Equal("World", metadata.Value);
		}
	}
}
