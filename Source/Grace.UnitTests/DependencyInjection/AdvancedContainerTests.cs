using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class AdvancedContainerTests
	{
		#region Metdata tests

		[Fact]
		public void LocateAllWithMetadataTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
			});
			var list = container.LocateAllWithMetadata<ISimpleObject>("Metadata");

			Assert.NotNull(list);

			Assert.Equal(5, list.Count());
		}

		[Fact]
		public void LocateAllWithMetadataFiltered()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
			});

			var list = container.LocateAllWithMetadata<ISimpleObject>("Metadata", "Group1");

			Assert.NotNull(list);

			Assert.Equal(3, list.Count());

		}

		#endregion

		#region Keyed tests

		[Fact]
		public void KeyedTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
										  c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
										  c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
										  c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
										  c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
									  });

			ISimpleObject simpleObject = container.Locate<ISimpleObject>(withKey: 3);

			Assert.NotNull(simpleObject);

			Assert.IsType<SimpleObjectC>(simpleObject);
		}

		[Fact]
		public void MultipleKeyLocate()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
			});

			ISimpleObject simpleObject = container.Locate<ISimpleObject>(withKey: new[] { 1, 2, 3 });

			Assert.NotNull(simpleObject);

			Assert.IsType<SimpleObjectC>(simpleObject);
		}

		[Fact]
		public void LocateAllKeyed()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
			});

			var simpleObjects = container.LocateAll<ISimpleObject>(withKey: new[] { 1, 2, 3 });

			Assert.NotNull(simpleObjects);
			Assert.Equal(3, simpleObjects.Count);
		}

		[Fact]
		public void LocateEmptyCollectionByKey()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
			});

			var simpleObjects = container.LocateAll<ISimpleObject>();

			Assert.NotNull(simpleObjects);
			Assert.Equal(0, simpleObjects.Count);
		}

		[Fact]
		public void LocateKeyedLazy()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
			});

			Lazy<ISimpleObject> simpleLazy = container.Locate<Lazy<ISimpleObject>>(withKey: 3);

			Assert.NotNull(simpleLazy);
			Assert.NotNull(simpleLazy.Value);
			Assert.IsType<SimpleObjectC>(simpleLazy.Value);
		}

		[Fact]
		public void LocateKeyedEnumerable()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
			});

			var list = container.Locate<IEnumerable<ISimpleObject>>(withKey: new[] { 3, 4, 5 });

			Assert.NotNull(list);

			Assert.Equal(3, list.Count());
		}

		[Fact]
		public void LocateKeyedMetadata()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1).WithMetadata("Test", "Group");
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2).WithMetadata("Test", "Group");
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3).WithMetadata("Test", "Group");
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4).WithMetadata("Test", "Group");
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5).WithMetadata("Test", "Group");
			});

			Meta<ISimpleObject> simpleMeta = container.Locate<Meta<ISimpleObject>>(withKey: 3);

			Assert.NotNull(simpleMeta);
			Assert.NotNull(simpleMeta.Value);
			Assert.IsType<SimpleObjectC>(simpleMeta.Value);
			Assert.Equal("Group", simpleMeta.Metadata["Test"]);
		}

		[Fact]
		public void LocateKeyedEnumerableLazy()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			{
				c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
				c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
				c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
				c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
				c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
			});

			var list = container.Locate<IEnumerable<Lazy<ISimpleObject>>>(withKey: new[] { 1, 2, 3 });

			Assert.NotNull(list);
			Assert.Equal(3, list.Count());

			foreach (Lazy<ISimpleObject> lazy in list)
			{
				Assert.NotNull(lazy.Value);
			}
		}

		#endregion

		#region new context

		[Fact]
		public void InNewContextTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<ContextSingleton>()
											.ByInterfaces()
											.UsingLifestyle(new SingletonPerInjectionContextLifestyle());
										  c.Export<ContextClassA>().ByInterfaces();
										  c.Export<ContextClassB>().ByInterfaces().InNewContext();
										  c.Export<ContextClassC>().ByInterfaces();
									  });

			IContextClassA classA = container.Locate<IContextClassA>();

			Assert.NotNull(classA);
			Assert.NotNull(classA.ContextClassB);
			Assert.NotNull(classA.ContextClassB.ContextClassC);

			Assert.NotNull(classA.ContextSingleton);
			Assert.NotNull(classA.ContextClassB.ContextSingleton);
			Assert.NotNull(classA.ContextClassB.ContextClassC.ContextSingleton);

			Assert.NotSame(classA.ContextSingleton, classA.ContextClassB.ContextSingleton);
			Assert.NotSame(classA.ContextSingleton, classA.ContextClassB.ContextClassC.ContextSingleton);
			Assert.Same(classA.ContextClassB.ContextSingleton, classA.ContextClassB.ContextClassC.ContextSingleton);

		}

		#endregion

		#region BeginLifetimeScope

		[Fact]
		public void BeginLifetimeScope()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().AndSingletonPerScope());

			IDisposableService service = container.Locate<IDisposableService>();

			Assert.NotNull(service);

			bool called = false;

			using (var scope = container.BeginLifetimeScope())
			{
				var secondService = scope.Locate<IDisposableService>();

				Assert.NotNull(secondService);
				Assert.NotSame(service, secondService);
				Assert.Same(secondService, scope.Locate<IDisposableService>());

				secondService.Disposing += (sender, args) => called = true;
			}

			Assert.True(called);
		}
		#endregion
	}
}
