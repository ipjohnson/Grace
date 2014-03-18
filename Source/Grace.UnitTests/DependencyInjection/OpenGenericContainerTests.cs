using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
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

		[Fact]
		public void LocateConcreteOpenGeneric()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(typeof(BaseGenericClass<,,,>)));

			var resolvedInstance = container.Locate<BaseGenericClass<int, string, double, DateTime>>();

			Assert.NotNull(resolvedInstance);
		}
		
		[Fact]
		public void ResolveConcretePartiallyClosedGeneric()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(typeof(PartialClosedClass<,,>)).As(typeof(BaseGenericClass<,,,>)));

			var resolvedInstance = container.Locate<BaseGenericClass<int, DateTime, string, double>>();

			Assert.NotNull(resolvedInstance);
		}

		[Fact]
		public void ResolveEvenMoreClosedGeneric()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(typeof(EvenMoreClosedClass<,>)).As(typeof(BaseGenericClass<,,,>)));

			var resolvedInstance = container.Locate<BaseGenericClass<int, DateTime, string, double>>();

			Assert.NotNull(resolvedInstance);
		}
	}
}
