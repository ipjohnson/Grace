using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public partial class ExportTypeSetTests
    {
        [Fact]
        public void ExportTypeSet_ByInterface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterface<IMultipleService>();
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);

            Assert.Throws<LocateException>(() => container.Locate<IBasicService>());
        }

        [Fact]
        public void ExportTypeSet_ByInterfaces()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces();
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        [Fact]
        public void ExportTypeSet_ByInterfaces_Filtered()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces(TypesThat.EndWith("MultipleService"));
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);

            Assert.Throws<LocateException>(() => container.Locate<IBasicService>());
        }

        [Fact]
        public void ExportTypeSet_Exclude()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces().Exclude(TypesThat.EndWith("4"));
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(4, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService5>(array[3]);
        }

        [Fact]
        public void ExportTypeSet_Lifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces().Lifestyle.Singleton();
            });

            var basicService = container.Locate<IBasicService>();
            Assert.Same(basicService, container.Locate<IBasicService>());

            var multipleService = container.Locate<IMultipleService>();
            Assert.Same(multipleService, container.Locate<IMultipleService>());

            Assert.Same(basicService, container.Locate<IBasicService>());
        }

        [Fact]
        public void ExportTypeSet_USingLifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces().UsingLifestyle(new SingletonLifestyle());
            });

            var basicService = container.Locate<IBasicService>();
            Assert.Same(basicService, container.Locate<IBasicService>());

            var multipleService = container.Locate<IMultipleService>();
            Assert.Same(multipleService, container.Locate<IMultipleService>());

            Assert.Same(basicService, container.Locate<IBasicService>());
        }

        public class GenericClass<T>
        {
            public GenericClass(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        [Fact]
        public void ExportTypeSet_When()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(new[] { typeof(MultipleService1) }).ByInterface<IMultipleService>().When.InjectedInto(typeof(DependentService<>));
                c.Export(new[] { typeof(MultipleService2) }).ByInterface<IMultipleService>().When.InjectedInto(typeof(GenericClass<>));
            });

            var instance = container.Locate<DependentService<IMultipleService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<MultipleService1>(instance.Value);

            var instance2 = container.Locate<GenericClass<IMultipleService>>();

            Assert.NotNull(instance2);
            Assert.NotNull(instance2.Value);
            Assert.IsType<MultipleService2>(instance2.Value);

        }

        [Fact]
        public void ExportTypeSet_BasedOn()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().BasedOn<CustomBaseService>().ByInterfaces();
            });

            var instance = container.Locate<IInheritingService>();

            Assert.NotNull(instance);
            Assert.IsType<InheritingClasses>(instance);

            Assert.Throws<LocateException>(() => container.Locate<IBasicService>());
        }
    }
}
