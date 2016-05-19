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

            IMultipleOutOfOrderOpenGeneric<DateTime, bool, string, IBasicService> multipleOut;

            container.TryLocate(out multipleOut);

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

		[Fact]
		public void MetaDataOnOpenGeneric()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(typeof(BaseGenericClass<,,,>)).WithMetadata("Hello", "World"));

			var resolvedInstance = container.Locate<Meta<BaseGenericClass<int, string, double, DateTime>>>();

			Assert.NotNull(resolvedInstance);
			Assert.NotNull(resolvedInstance.Value);

			Assert.Equal("World", resolvedInstance.Metadata["Hello"]);
		}

		[Fact]
		public void EnrichmentDelegateOnOpenGeneric()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			bool enrichmentCalled = false;

			container.Configure(c => c.Export(typeof(BaseGenericClass<,,,>)).EnrichWith((scope, context, x) =>
																												 {
																													 enrichmentCalled = true;
																													 return x;
																												 }));

			var genericObject = container.Locate<BaseGenericClass<int, string, double, DateTime>>();

			Assert.NotNull(genericObject);

			Assert.True(enrichmentCalled);
		}

		[Fact]
		public void PrioritizeSemiClosedGenerics()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export(typeof(BaseGenericClass<,,,>));
										  c.Export(typeof(PartialClosedClass<,,>)).As(typeof(BaseGenericClass<,,,>));
										  c.Export(typeof(EvenMoreClosedClass<,>)).As(typeof(BaseGenericClass<,,,>));
										  c.PrioritizePartiallyClosedGenerics();
									  });

			var openObject = container.Locate<BaseGenericClass<int, string, double, DateTime>>();

			Assert.NotNull(openObject);

			var semiClosed = container.Locate<BaseGenericClass<int, string, double, double>>();

			Assert.NotNull(semiClosed);
			Assert.IsType<PartialClosedClass<int, string, double>>(semiClosed);

			var reallyClosed = container.Locate<BaseGenericClass<int, string, string, double>>();

			Assert.NotNull(reallyClosed);
			Assert.IsType<EvenMoreClosedClass<int, string>>(reallyClosed);
		}

		[Fact]
		public void BulkRegisterOpenGenericClasses()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).
																 BasedOn(typeof(BaseGenericClass<,,,>)).
																 PrioritizePartiallyClosedGenerics());

			var openObject = container.Locate<BaseGenericClass<int, string, double, DateTime>>();

			Assert.NotNull(openObject);

			var semiClosed = container.Locate<BaseGenericClass<int, string, double, double>>();

			Assert.NotNull(semiClosed);
			Assert.IsType<PartialClosedClass<int, string, double>>(semiClosed);

			var reallyClosed = container.Locate<BaseGenericClass<int, string, string, double>>();

			Assert.NotNull(reallyClosed);
			Assert.IsType<EvenMoreClosedClass<int, string>>(reallyClosed);
		}


		[Fact]
		public void BulkRegisterOpenGenericByInterface()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).
																 ByInterface(typeof(IOpenGenericPartiallyClosedInterface<,,,>)).
																 PrioritizePartiallyClosedGenerics());

			var openImpl = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, float>>();

			Assert.NotNull(openImpl);

			var partiallyClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, string>>();

			Assert.NotNull(partiallyClosed);
			Assert.IsType<PartiallyClosedInterface<string, int, DateTime>>(partiallyClosed);

			var moreClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, double, string>>();

			Assert.NotNull(moreClosed);
			Assert.IsType<EvenMoreClosedInterface<string, int>>(moreClosed);


		}

		[Fact]
		public void BulkRegisterOpenGenericByInterfaces()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).
																 ByInterfaces().
																 PrioritizePartiallyClosedGenerics());

			var openImpl = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, float>>();

			Assert.NotNull(openImpl);

			var partiallyClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, string>>();

			Assert.NotNull(partiallyClosed);
			Assert.IsType<PartiallyClosedInterface<string, int, DateTime>>(partiallyClosed);

			var moreClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, double, string>>();

			Assert.NotNull(moreClosed);
			Assert.IsType<EvenMoreClosedInterface<string, int>>(moreClosed);


		}
	}
}
