using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class LazyWrapperTests
    {
        [Fact]
        public void Lazy_Array()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
            });

            var array = container.Locate<Lazy<IMultipleService>[]>();

            Assert.Equal(5, array.Length);

            Assert.IsType<MultipleService1>(array[0].Value);
            Assert.IsType<MultipleService2>(array[1].Value);
            Assert.IsType<MultipleService3>(array[2].Value);
            Assert.IsType<MultipleService4>(array[3].Value);
            Assert.IsType<MultipleService5>(array[4].Value);
        }


        public class LazyType1
        {
            private Lazy<LazyType2> _lazy;

            public LazyType1(Lazy<LazyType2> lazy)
            {
                _lazy = lazy;
            }

            public LazyType2 LazyType2 => _lazy.Value;
        }

        public class LazyType2
        {
            private Lazy<LazyType1> _lazy;

            public LazyType2(Lazy<LazyType1> lazy)
            {
                _lazy = lazy;
            }

            public LazyType1 LazyType1 => _lazy.Value;
        }


        [Fact]
        public void Lazy_Recursive_SingletonPerObjectGraph()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<LazyType1>().Lifestyle.SingletonPerObjectGraph();
                c.Export<LazyType2>();
            });

            var instance = container.Locate<LazyType1>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.LazyType2);
            Assert.Same(instance, instance.LazyType2.LazyType1);
        }

        [Fact]
        public void Lazy_Recursive_Singleton()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<LazyType1>().Lifestyle.Singleton();
                c.Export<LazyType2>().Lifestyle.Singleton();
            });

            var instance = container.Locate<LazyType1>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.LazyType2);
            Assert.Same(instance, instance.LazyType2.LazyType1);
        }
    }
}
