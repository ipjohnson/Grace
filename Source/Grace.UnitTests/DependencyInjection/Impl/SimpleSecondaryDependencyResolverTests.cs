using System.Collections.Generic;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class SimpleSecondaryDependencyResolverTests
	{
		[Fact]
		public void InitValueTest()
		{
			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator(
				new Dictionary<string, object>
				{
					{ "BasicService", new BasicService() }
				});

			object returnValue =
				resolver.Locate(new FauxInjectionScope(), new FauxInjectionContext(), "BasicService", null, null, null);

			Assert.NotNull(returnValue);
			Assert.IsType(typeof(BasicService), returnValue);
		}

		[Fact]
		public void RegisterInstanceByTypeTest()
		{
			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator();

			resolver.AddResolveValue(new BasicService());

			object returnValue =
				resolver.Locate(new FauxInjectionScope(), new FauxInjectionContext(), null, typeof(BasicService), null, null);

			Assert.NotNull(returnValue);
			Assert.IsType(typeof(BasicService), returnValue);
		}

		[Fact]
		public void RegisterFuncByTypeTest()
		{
			SimpleSecondaryExportLocator resolver = new SimpleSecondaryExportLocator();

			resolver.AddResolveValue(() => new BasicService());

			object returnValue =
				resolver.Locate(new FauxInjectionScope(), new FauxInjectionContext(), null, typeof(BasicService), null, null);

			Assert.NotNull(returnValue);
			Assert.IsType(typeof(BasicService), returnValue);
		}
	}
}