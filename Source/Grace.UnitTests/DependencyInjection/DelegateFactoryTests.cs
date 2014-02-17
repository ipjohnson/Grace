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
	public class DelegateFactoryTests
	{
		public delegate IBasicService BasicServiceDelegate();

		public delegate ISomePropertyService SomePropertyServiceWithString(string testString);

		public delegate ISomePropertyService SomePropertyServiceWithNone();

		public delegate ISomePropertyService SomePropertyServiceWithBasicService(IBasicService basicService);

		[Fact]
		public void BasicServiceFactoryTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>());

			BasicServiceDelegate bsDelegate = container.Locate<BasicServiceDelegate>();

			Assert.NotNull(bsDelegate);

			IBasicService basicService = bsDelegate();

			Assert.NotNull(basicService);
		}

		[Fact]
		public void FactorySomePropertyServiceWithString()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<StringArgSomePropertyService>().As<ISomePropertyService>());

			SomePropertyServiceWithString factory =
				container.Locate<SomePropertyServiceWithString>();

			Assert.NotNull(factory);

			ISomePropertyService service = factory("Hello World");

			Assert.NotNull(service);
			Assert.Equal("Hello World", service.SomeProperty);
		}

		[Fact]
		public void FactorySomePropertyServiceWithNone()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<ReferenceArgSomePropertyService>().As<ISomePropertyService>();
										  c.Export<BasicService>().As<IBasicService>();
									  });

			SomePropertyServiceWithNone factory =
				container.Locate<SomePropertyServiceWithNone>();

			Assert.NotNull(factory);

			ISomePropertyService service = factory();

			Assert.NotNull(service);
			Assert.NotNull(service.SomeProperty);
		}

		[Fact]
		public void FactorySomePropertyServiceWithBasicService()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<ReferenceArgSomePropertyService>().As<ISomePropertyService>();
										  c.Export<BasicService>().As<IBasicService>();
									  });

			SomePropertyServiceWithBasicService factory =
				container.Locate<SomePropertyServiceWithBasicService>();

			Assert.NotNull(factory);

			BasicService newBasicService = new BasicService();

			ISomePropertyService service = factory(newBasicService);

			Assert.NotNull(service);
			Assert.Same(newBasicService, service.SomeProperty);
		}

		[Fact]
		public void FactoryOneArgWithStringTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<OneArgStringParameterService>().As<IArrayOfObjectsPropertyService>());

			var factory = container.Locate<OneArgStringParameterService.Activate>();

			Assert.NotNull(factory);

			var instance = factory("Blah");

			Assert.NotNull(instance);
			Assert.Equal(1, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
		}

		[Fact]
		public void FactoryOneArgWithBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<OneArgRefParameterService>().As<IArrayOfObjectsPropertyService>());

			var factory = container.Locate<OneArgRefParameterService.ActivateWithBasicService>();

			Assert.NotNull(factory);

			var basicService = new BasicService();

			var instance = factory(basicService);

			Assert.NotNull(instance);
			Assert.Equal(1, instance.Parameters.Length);
			Assert.Equal(basicService, instance.Parameters[0]);
		}

		[Fact]
		public void FactoryTwoArgWithBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<TwoArgParameterService>().As<IArrayOfObjectsPropertyService>());

			var factory = container.Locate<TwoArgParameterService.ActivateWithBasicService>();

			Assert.NotNull(factory);

			var basicService = new BasicService();

			var instance = factory("Blah", basicService);

			Assert.NotNull(instance);

			Assert.Equal(2, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(basicService, instance.Parameters[1]);
		}

		[Fact]
		public void FactoryTwoArgWithoutBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
									  {
										  c.Export<TwoArgParameterService>().As<IArrayOfObjectsPropertyService>();
										  c.ExportInstance(basicService).As<IBasicService>();
									  });

			var factory = container.Locate<TwoArgParameterService.ActivateWithOutBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah");

			Assert.NotNull(instance);

			Assert.Equal(2, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(basicService, instance.Parameters[1]);
		}

		[Fact]
		public void FactoryThreeWithBasicServiceTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c => c.Export<ThreeArgParameterService>().As<IArrayOfObjectsPropertyService>());

			var factory = container.Locate<ThreeArgParameterService.ActivateWithBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah", 5, basicService);

			Assert.NotNull(instance);

			Assert.Equal(3, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(basicService, instance.Parameters[2]);
		}

		[Fact]
		public void FactoryThreeWithOutBasicServiceTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
									  {
										  c.Export<ThreeArgParameterService>().As<IArrayOfObjectsPropertyService>();
										  c.ExportInstance(basicService).As<IBasicService>();
									  });

			var factory = container.Locate<ThreeArgParameterService.ActivateWithOutBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah", 5);

			Assert.NotNull(instance);

			Assert.Equal(3, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(basicService, instance.Parameters[2]);
		}

		[Fact]
		public void FactoryThreeWithOutBasicServiceOutOfOrderTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
			{
				c.Export<ThreeArgParameterService>().As<IArrayOfObjectsPropertyService>();
				c.ExportInstance(basicService).As<IBasicService>();
			});

			var factory = container.Locate<ThreeArgParameterService.ActivateWithOutBasicServiceAndOutOfOrder>();

			Assert.NotNull(factory);

			var instance = factory(5, "Blah");

			Assert.NotNull(instance);

			Assert.Equal(3, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(basicService, instance.Parameters[2]);
		}

		[Fact]
		public void FactoryFourArgWithBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c => c.Export<FourArgParameterService>().As<IArrayOfObjectsPropertyService>());

			var factory = container.Locate<FourArgParameterService.ActivateWithBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah", 5, 9.0, basicService);

			Assert.NotNull(instance);
			Assert.Equal(4, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(9.0, instance.Parameters[2]);
			Assert.Equal(basicService, instance.Parameters[3]);
		}

		[Fact]
		public void FactoryFourArgWithoutBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
			{
				c.Export<FourArgParameterService>().As<IArrayOfObjectsPropertyService>();
				c.ExportInstance(basicService).As<IBasicService>();
			});

			var factory = container.Locate<FourArgParameterService.ActivateWithOutBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah", 5, 9.0);

			Assert.NotNull(instance);
			Assert.Equal(4, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(9.0, instance.Parameters[2]);
			Assert.Equal(basicService, instance.Parameters[3]);

		}

		[Fact]
		public void FactoryFourArgWithoutBasicAndOutOfOrderTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
			{
				c.Export<FourArgParameterService>().As<IArrayOfObjectsPropertyService>();
				c.ExportInstance(basicService).As<IBasicService>();
			});

			var factory = container.Locate<FourArgParameterService.ActivateWithOutBasicServiceAndOutOfOrder>();

			Assert.NotNull(factory);

			var instance = factory(9.0, 5, "Blah");

			Assert.NotNull(instance);
			Assert.Equal(4, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(9.0, instance.Parameters[2]);
			Assert.Equal(basicService, instance.Parameters[3]);
		}

		[Fact]
		public void FactoryFiveArgWithBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c => c.Export<FiveArgParameterService>().As<IArrayOfObjectsPropertyService>());

			var factory = container.Locate<FiveArgParameterService.ActivateWithBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah", 5, 9.0, 14.0m, basicService);

			Assert.NotNull(instance);
			Assert.Equal(5, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(9.0, instance.Parameters[2]);
			Assert.Equal(14.0m, instance.Parameters[3]);
			Assert.Equal(basicService, instance.Parameters[4]);
		}

		[Fact]
		public void FactoryFiveArgWithOutBasicTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
			                    {
				                    c.Export<FiveArgParameterService>().As<IArrayOfObjectsPropertyService>();
				                    c.ExportInstance(basicService).As<IBasicService>();
			                    });

			var factory = container.Locate<FiveArgParameterService.ActivateWithOutBasicService>();

			Assert.NotNull(factory);

			var instance = factory("Blah", 5, 9.0, 14.0m);

			Assert.NotNull(instance);
			Assert.Equal(5, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(9.0, instance.Parameters[2]);
			Assert.Equal(14.0m, instance.Parameters[3]);
			Assert.Equal(basicService, instance.Parameters[4]);
		}

		[Fact]
		public void FactoryFiveArgWithOutBasicAndOutOfOrderTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			var basicService = new BasicService();

			container.Configure(c =>
			{
				c.Export<FiveArgParameterService>().As<IArrayOfObjectsPropertyService>();
				c.ExportInstance(basicService).As<IBasicService>();
			});

			var factory = container.Locate<FiveArgParameterService.ActivateWithOutBasicServiceAndOutOfOrder>();

			Assert.NotNull(factory);

			var instance = factory(14.0m,"Blah",9.0,5);

			Assert.NotNull(instance);
			Assert.Equal(5, instance.Parameters.Length);
			Assert.Equal("Blah", instance.Parameters[0]);
			Assert.Equal(5, instance.Parameters[1]);
			Assert.Equal(9.0, instance.Parameters[2]);
			Assert.Equal(14.0m, instance.Parameters[3]);
			Assert.Equal(basicService, instance.Parameters[4]);
		}
	}
}
