using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using Grace.Tests.DependencyInjection.AddOns;
using Xunit;

#if NET5_0
using System.Threading.Tasks;
#endif

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


        [Fact]
        public void ExportTypeSet_WithInspector()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>()
                    .ByInterfaces()
                    .WithInspector(new StrategyInspectorTests.StrategyInspectorInjectProperty());
            });

            var instance = container.Locate<IPropertyInjectionService>(new { Count = 5 });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
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
        public void ExportTypeSet_BasedOn_Generic()
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

        [Fact]
        public void ExportTypeSet_BasedOn()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().BasedOn(typeof(CustomBaseService)).ByInterfaces();
            });

            var instance = container.Locate<IInheritingService>();

            Assert.NotNull(instance);
            Assert.IsType<InheritingClasses>(instance);

            Assert.Throws<LocateException>(() => container.Locate<IBasicService>());
        }

        [Fact]
        public void ExportTypeSet_ByType()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByType().Lifestyle.Singleton();
            });

            var instance = container.Locate<BasicService>();
            var instance2 = container.Locate<BasicService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance2);
            Assert.Same(instance, instance2);
        }

        [Fact]
        public void ExportTypeSet_Commands()
        {
            var container = new DependencyInjectionContainer(c => c.SupportFuncType = false);

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<CommandA>().ByInterface(typeof(ICommand<>));
            });

            var exports = container.StrategyCollectionContainer.GetAllStrategies().ToList();

            Assert.Equal(6, exports.Count);
        }

        [Fact]
        public void ExportTypeSet_ByType_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByTypes(t =>
                {
                    if (t.Name.StartsWith("Multiple"))
                    {
                        return new[] { t };
                    }

                    return new Type[0];
                }).Lifestyle.Singleton();
            });

            var instance = container.Locate<BasicService>();
            var instance2 = container.Locate<BasicService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance2);
            Assert.NotSame(instance, instance2);

            var multiple = container.Locate<MultipleService1>();
            var multiple2 = container.Locate<MultipleService1>();

            Assert.NotNull(multiple);
            Assert.NotNull(multiple2);
            Assert.Same(multiple, multiple2);
        }


        [Fact]
        public void ExportTypeSet_ByTypeKeyed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByKeyedTypes(t =>
                {
                    if (t.Name.StartsWith("Multiple"))
                    {
                        return new[] { new Tuple<Type, object>(typeof(IMultipleService), t.Name.Last()) };
                    }

                    return new Tuple<Type, object>[0];
                });
            });

            var instance1 = container.Locate<IMultipleService>(withKey: '1');

            Assert.NotNull(instance1);
        }

#if NET5_0
        [Fact]
        public async Task ExportTypeSet_ExternallyOwned()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>()
                    .ByInterfaces();
            });

            var disposed = false;

            await using (var scope = container.BeginLifetimeScope())
            {
                var disposable = scope.Locate<IDisposableService>();

                disposable.Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);

            container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces().ExternallyOwned();
            });

            disposed = false;

            await using (var scope = container.BeginLifetimeScope())
            {
                var disposable = scope.Locate<IDisposableService>();

                disposable.Disposing += (sender, args) => disposed = true;
            }

            Assert.False(disposed);
        }
#else
        [Fact]
        public void ExportTypeSet_ExternallyOwned()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>()
                    .ByInterfaces();
            });

            var disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var disposable = scope.Locate<IDisposableService>();

                disposable.Disposing += (sender, args) => disposed = true;
            }

            Assert.True(disposed);

            container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<IMultipleService>().ByInterfaces().ExternallyOwned();
            });

            disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var disposable = scope.Locate<IDisposableService>();

                disposable.Disposing += (sender, args) => disposed = true;
            }

            Assert.False(disposed);
        }
#endif

        [Fact]
        public void ExportTypeSet_UsingLifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
                {
                    c.Export(new[] { typeof(BasicService), typeof(CustomBasicService) }).ByInterface<IBasicService>()
                        .UsingLifestyle(CustomLifestylePicker);
                });

            var instances = container.Locate<List<IBasicService>>();

            Assert.Equal(2, instances.Count);

            var basicInstance = instances.First(i => i is BasicService);
            var customInstance = instances.First(i => i is CustomBasicService);

            var instances2 = container.Locate<List<IBasicService>>();

            Assert.Equal(2, instances2.Count);

            var basicInstance2 = instances2.First(i => i is BasicService);
            var customInstance2 = instances2.First(i => i is CustomBasicService);

            Assert.Same(basicInstance, basicInstance2);
            Assert.NotSame(customInstance, customInstance2);
        }

        private ICompiledLifestyle CustomLifestylePicker(Type arg)
        {
            if (arg == typeof(BasicService))
            {
                return new SingletonLifestyle();
            }

            return null;
        }
    }
}

