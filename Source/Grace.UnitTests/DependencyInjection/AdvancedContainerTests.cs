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
		[Fact]
		public void InNewContextTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c =>
									  {
										  c.Export<ContextSingleton>()
											.ByInterfaces()
											.UsingLifestyleContainer(new SingletonPerInjectionContextLifestyle());
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
	}
}
