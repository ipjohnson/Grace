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
	public class MetaTests
	{
		[Fact]
		public void MetaSimpleTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Hello", "World"));

			var metaObject = container.Locate<Meta<ISimpleObject>>();

			Assert.NotNull(metaObject);
			Assert.NotNull(metaObject.Value);
			Assert.NotNull(metaObject.Metadata);

			var metadata = metaObject.Metadata.First();

			Assert.Equal("Hello", metadata.Key);
			Assert.Equal("World", metadata.Value);
		}

		[Fact]
		public void MetaCollectionTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
			                    {
										  c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Hello", "World");
										  c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Hello", "World");
										  c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Hello", "World");
										  c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Hello", "World");
										  c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Hello", "World");
			                    });

			Meta<ISimpleObject>[] metaArray = container.Locate<Meta<ISimpleObject>[]>();

			Assert.Equal(5, metaArray.Length);

			foreach (Meta<ISimpleObject> meta in metaArray)
			{
				var metadata = meta.Metadata.First();

				Assert.Equal("Hello", metadata.Key);
				Assert.Equal("World", metadata.Value);

				Assert.NotNull(meta.Value);
			}
		}
	}
}
