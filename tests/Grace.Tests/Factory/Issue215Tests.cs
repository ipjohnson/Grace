using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Grace.DependencyInjection;
using Grace.Factory;
using Xunit;

namespace Grace.Tests.Factory
{
    public class Issue215Tests
    {
        [Fact]
        public void Issue215Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => { c.ExportFactory<IExampleClassFactory>(); });

            var factory = container.Locate<IExampleClassFactory>();

            var instance = factory.Create(1, 2, true, 3, 4, new CancellationTokenSource());
        }

        public interface IExampleClassFactory
        {
            ExampleClass Create(
                long startId,
                long endId,
                bool reverse,
                int loadIdsLimit,
                long maxBufferSize,
                CancellationTokenSource cancellationTokenSource);
        }

        public class ExampleClass
        {
            public ExampleClass(
                long startId,
                long endId,
                bool reverse,
                int loadIdsLimit,
                long maxBufferSize,
                CancellationTokenSource cancellationTokenSource,
                Dependency1 dependency1,
                Dependency2 dependency2)
            {
                Assert.Equal(1, startId);
                Assert.Equal(2, endId);
                Assert.True(reverse);
                Assert.Equal(3,loadIdsLimit);
                Assert.Equal(4, maxBufferSize);
                Assert.NotNull(cancellationTokenSource);
                Assert.NotNull(dependency1);
                Assert.NotNull(dependency2);
            }
        }


        public class Dependency1
        {

        }

        public class Dependency2
        {

        }
    }
}
