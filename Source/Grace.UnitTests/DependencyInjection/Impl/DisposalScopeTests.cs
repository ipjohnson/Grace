using System;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class DisposalScopeTests
	{
		[Fact]
		public void BasicDisposalTest()
		{
			DisposalScope disposalScope = new DisposalScope();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			disposalScope.AddDisposable(disposableService);

			disposalScope.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void ThrowExceptionDuringCleanUp()
		{
			DisposalScope disposalScope = new DisposalScope();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			disposalScope.AddDisposable(disposableService, disposed => { throw new Exception(); });

			disposalScope.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void ThrowExceptionDuringDispose()
		{
			DisposalScope disposalScope = new DisposalScope();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) =>
			                               {
				                               eventFired = true;
				                               throw new Exception();
			                               };

			disposalScope.AddDisposable(disposableService, disposed => { throw new Exception(); });

			disposalScope.Dispose();

			Assert.True(eventFired);
		}

		[Fact]
		public void DoubleDisposeTest()
		{
			DisposalScope disposalScope = new DisposalScope();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			disposalScope.AddDisposable(disposableService);

			disposalScope.Dispose();

			Assert.True(eventFired);

			eventFired = false;

			disposalScope.Dispose();

			Assert.False(eventFired);
		}

		[Fact]
		public void CleanUpDelegateTest()
		{
			DisposalScope disposalScope = new DisposalScope();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;
			bool cleanUpCalled = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			disposalScope.AddDisposable(disposableService,
				disposed =>
				{
					Assert.True(ReferenceEquals(disposableService, disposed));

					cleanUpCalled = true;
				});

			disposalScope.Dispose();

			Assert.True(eventFired);
			Assert.True(cleanUpCalled);
		}

		[Fact]
		public void RemoveFromDisposalTest()
		{
			DisposalScope disposalScope = new DisposalScope();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			disposalScope.AddDisposable(disposableService);

			disposalScope.RemoveDisposable(disposableService);

			disposalScope.Dispose();

			Assert.False(eventFired);
		}
	}
}