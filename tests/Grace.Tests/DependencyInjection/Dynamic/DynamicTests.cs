using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Grace.Tests.DependencyInjection.Enumerable;
using Xunit;

namespace Grace.Tests.DependencyInjection.Dynamic
{
    public class DynamicTests
    {
        [Fact]
        public void Dynamic_Constructor_Parameter_Missing_Throws()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().IsDynamic());

            using (var childScope = container.CreateChildScope())
            {
                Assert.Throws<LocateException>(() => childScope.Locate<IDependentService<IBasicService>>());

                childScope.Configure(c => c.Export<BasicService>().As<IBasicService>());

                var instance = childScope.Locate<IDependentService<IBasicService>>();

                Assert.NotNull(instance);
                Assert.NotNull(instance.Value);
            }
        }

        [Fact]
        public void Dynamic_Constructor_Parameter_Not_Required()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DependentService<int>>().As<IDependentService<int>>().WithCtorParam<int>().IsRequired(false).IsDynamic());

            var instance = container.Locate<IDependentService<int>>();

            Assert.NotNull(instance);
            Assert.Equal(0, instance.Value);
        }

        [Fact]
        public void Dynamic_Constructor_Parameter_Returns_Default()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService { Count = 10 };

            container.Configure(
                c =>
                    c.Export<DependentService<IBasicService>>()
                        .As<IDependentService<IBasicService>>()
                        .WithCtorParam<IBasicService>()
                        .IsDynamic()
                        .DefaultValue(basicService));

            using (var childScope = container.CreateChildScope())
            {
                var instance = childScope.Locate<IDependentService<IBasicService>>();

                Assert.NotNull(instance);
                Assert.NotNull(instance.Value);
                Assert.Equal(10, instance.Value.Count);

                childScope.Configure(c => c.Export<BasicService>().As<IBasicService>().ImportProperty(b => b.Count).DefaultValue(20));

                instance = childScope.Locate<IDependentService<IBasicService>>();

                Assert.NotNull(instance);
                Assert.NotNull(instance.Value);
                Assert.Equal(20, instance.Value.Count);
            }
        }
        
        [Fact]
        public void Dynamic_Constructor_Parameter_Resolve_From_Child_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().IsDynamic());

            using (var childScope = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>()))
            {
                var instance = childScope.Locate<IDependentService<IBasicService>>();

                Assert.NotNull(instance);
                Assert.NotNull(instance.Value);
            }
        }

        [Fact]
        public void Dynamic_Parameter_Resolve_IEnumerable()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().IsDynamic());

            var instance = container.Locate<IDependentService<IEnumerable<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.False(instance.Value.Any());

            container.Configure(c => c.Export<MultipleService1>().As<IMultipleService>());

            instance = container.Locate<IDependentService<IEnumerable<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Single(instance.Value);
        }


        [Fact]
        public void Dynamic_Parameter_Resolve_IEnumerable_With_Creator()
        {
            var container = new DependencyInjectionContainer(configuration =>
                configuration.Behaviors.CustomEnumerableCreator = new EnumerableTests.ReadOnlyCreator());

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().IsDynamic());

            var instance = container.Locate<IDependentService<IEnumerable<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<ReadOnlyCollection<IMultipleService>>(instance.Value);
            Assert.False(instance.Value.Any());

            container.Configure(c => c.Export<MultipleService1>().As<IMultipleService>());

            instance = container.Locate<IDependentService<IEnumerable<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<ReadOnlyCollection<IMultipleService>>(instance.Value);
            Assert.Single(instance.Value);
        }


        [Fact]
        public void Dynamic_Parameter_Resolve_Array()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().IsDynamic());

            var instance = container.Locate<IDependentService<IMultipleService[]>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.False(instance.Value.Any());

            container.Configure(c => c.Export<MultipleService1>().As<IMultipleService>());

            instance = container.Locate<IDependentService<IMultipleService[]>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Single(instance.Value);
        }
    }
}
