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

            var cancellationToken = new CancellationTokenSource();

            var instance = factory.Create(1, 2, true, 3, 4, cancellationToken);

            Assert.NotNull(instance);

            Assert.Equal(1, instance.StartId);
            Assert.Equal(2, instance.EndId);
            Assert.True(instance.Reverse);
            Assert.Equal(3, instance.LoadIdsLimit);
            Assert.Equal(4, instance.MaxBufferSize);
            Assert.Same(cancellationToken, instance.CancellationTokenSource);
            Assert.NotNull(instance.Dependency1);
            Assert.NotNull(instance.Dependency2);
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
                StartId = startId;
                EndId = endId;
                Reverse = reverse;
                LoadIdsLimit = loadIdsLimit;
                MaxBufferSize = maxBufferSize;
                CancellationTokenSource = cancellationTokenSource;
                Dependency1 = dependency1;
                Dependency2 = dependency2;
            }

            public long StartId { get; }

            public long EndId { get; }

            public bool Reverse { get; }

            public int LoadIdsLimit { get; }

            public long MaxBufferSize { get; }

            public CancellationTokenSource CancellationTokenSource { get; }

            public Dependency1 Dependency1 { get; }

            public Dependency2 Dependency2 { get; }

        }


        public class Dependency1
        {

        }

        public class Dependency2
        {

        }
    }
}
