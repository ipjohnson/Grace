using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class OwnedTests
	{
		[Fact]
		public void SimpleNonDisposableTest()
		{
			Owned<IBasicService> owned = new Owned<IBasicService>();
			BasicService basicService = new BasicService();

			owned.SetValue(basicService);

			Assert.True(ReferenceEquals(owned.Value, basicService));

			owned.Dispose();
		}

		[Fact]
		public void SimpleDisposableTest()
		{
			Owned<IDisposableService> owned = new Owned<IDisposableService>();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			owned.SetValue(disposableService);

			Assert.True(ReferenceEquals(owned.Value, disposableService));

			owned.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void OwnedCollection()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

			List<Owned<IDisposableService>> ownedList = 
				container.LocateAll<Owned<IDisposableService>>();

			Assert.NotNull(ownedList);
			Assert.Equal(1, ownedList.Count);
			Assert.NotNull(ownedList[0].Value);

			bool disposedCalled = false;

			ownedList[0].Value.Disposing += (sender, args) => disposedCalled = true;

			ownedList[0].Dispose();

			Assert.True(disposedCalled);
		}

		[Fact]
		public void OwnedLazyTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

			Owned<Lazy<IDisposableService>> ownedLazy =
				container.Locate<Owned<Lazy<IDisposableService>>>();

			Assert.NotNull(ownedLazy);
			Assert.NotNull(ownedLazy.Value);
			Assert.NotNull(ownedLazy.Value.Value);
			
			bool disposedCalled = false;

			ownedLazy.Value.Value.Disposing += (sender, args) => disposedCalled = true;

			ownedLazy.Dispose();

			Assert.True(disposedCalled);
		}

		[Fact]
		public void OwnedLazyMetaTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().WithMetadata(
				"Hello","World"));

			Owned<Lazy<Meta<IDisposableService>>> ownedLazy =
				container.Locate <Owned<Lazy<Meta<IDisposableService>>>>();

			Assert.NotNull(ownedLazy);
			Assert.NotNull(ownedLazy.Value);
			Assert.NotNull(ownedLazy.Value.Value);
			Assert.NotNull(ownedLazy.Value.Value.Value);

			bool disposedCalled = false;

			ownedLazy.Value.Value.Value.Disposing += (sender, args) => disposedCalled = true;

			ownedLazy.Dispose();

			Assert.True(disposedCalled);

			var metadata = ownedLazy.Value.Value.Metadata.First();

			Assert.Equal("Hello",metadata.Key);
			Assert.Equal("World",metadata.Value);
		}

	}
}