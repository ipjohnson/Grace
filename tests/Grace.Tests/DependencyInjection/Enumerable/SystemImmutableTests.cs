using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class SystemImmutableTests
    {
        [Fact]
        public void System_ImmutableList_Test()
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

            var list = container.Locate<ImmutableList<IMultipleService>>();

            Assert.Equal(5, list.Count);
            Assert.IsType<MultipleService1>(list[0]);
            Assert.IsType<MultipleService2>(list[1]);
            Assert.IsType<MultipleService3>(list[2]);
            Assert.IsType<MultipleService4>(list[3]);
            Assert.IsType<MultipleService5>(list[4]);
        }

        [Fact]
        public void System_ImmutableList_Dependency_Test()
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

            var service = container.Locate<DependentService<ImmutableList<IMultipleService>>>();

            var list = service.Value;

            Assert.Equal(5, list.Count);
            Assert.IsType<MultipleService1>(list[0]);
            Assert.IsType<MultipleService2>(list[1]);
            Assert.IsType<MultipleService3>(list[2]);
            Assert.IsType<MultipleService4>(list[3]);
            Assert.IsType<MultipleService5>(list[4]);
        }

        [Fact]
        public void System_ImmutableArray_Test()
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

            var array = container.Locate<ImmutableArray<IMultipleService>>();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        [Fact]
        public void System_ImmutableArray_Dependency_Test()
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

            var service = container.Locate<DependentService<ImmutableArray<IMultipleService>>>();

            var array = service.Value;

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }
        
        [Fact]
        public void System_ImmutableQueue_Test()
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

            var array = container.Locate<ImmutableArray<IMultipleService>>().Select(t => t).ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        [Fact]
        public void System_ImmutableQueue_Dependency_Test()
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

            var service = container.Locate<DependentService<ImmutableQueue<IMultipleService>>>();

            var array = service.Value.Select(t => t).ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }


        [Fact]
        public void System_ImmutableStack_Test()
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

            var array = container.Locate<ImmutableStack<IMultipleService>>().Select(t => t).ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService5>(array[0]);
            Assert.IsType<MultipleService4>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService2>(array[3]);
            Assert.IsType<MultipleService1>(array[4]);
        }

        [Fact]
        public void System_ImmutableStack_Dependency_Test()
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

            var service = container.Locate<DependentService<ImmutableStack<IMultipleService>>>();

            var array = service.Value.Select(t => t).ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService5>(array[0]);
            Assert.IsType<MultipleService4>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService2>(array[3]);
            Assert.IsType<MultipleService1>(array[4]);
        }
    }
}
