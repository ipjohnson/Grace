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
	public class OpenGenericContainerTests
	{
		[Fact]
		public void MultipleOutOfOrderOpenGeneric()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export(typeof(MultipleOutOfOrderOpenGeneric<,,,>)).ByInterfaces();
										  c.Export<BasicService>().ByInterfaces();
									  });

			IMultipleOutOfOrderOpenGeneric<IBasicService, bool, string, DateTime> multipleOut =
				container.Locate<IMultipleOutOfOrderOpenGeneric<IBasicService, bool, string, DateTime>>();

			Assert.NotNull(multipleOut);
		}

		[Fact]
		public void MissingConstraintOnOutOfOrderOpen()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
											{
												c.Export(typeof(MultipleOutOfOrderOpenGeneric<,,,>)).ByInterfaces();
												c.Export<BasicService>().ByInterfaces();
											});

			var multipleOut =
				container.Locate<IMultipleOutOfOrderOpenGeneric<DateTime, bool, string, IBasicService>>();

			Assert.Null(multipleOut);

		}
	}
}
