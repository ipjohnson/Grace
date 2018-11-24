using System;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Generics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{

    
    public class SimpleGenericTests
    {
        [Fact]
        public void Open_Generic_Locate_Generic_Basic_Service()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(ImportGenericService<>)).As(typeof(IImportGenericService<>));
                c.Export<BasicService>().As<IBasicService>();
            });

            var service = container.Locate<IImportGenericService<IBasicService>>();

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
            Assert.IsType<BasicService>(service.Value);
        }

        [Fact]
        public void Open_Generic_Locate_Generic_Int_Service()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(ImportGenericService<>)).As(typeof(IImportGenericService<>)));

            var service = container.Locate<IImportGenericService<int>>(new { value = 5 });

            Assert.NotNull(service);
            Assert.Equal(5, service.Value);
        }

        public interface IGenericClass<T>
        {
            T Value { get; set; }
        }

        public class GenericClassA<T> : IGenericClass<T>
        {
            public T Value { get; set; }
        }

        public class GenericClassB<T> : IGenericClass<T>
        {
            public T Value { get; set; }
        }

        public class GenericClassC<T> : IGenericClass<T>
        {
            public T Value { get; set; }
        }

        [Fact]
        public void Locate_Open_Generic_With_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).WithCtorParam<object>().Named("value").LocateWithKey('A');
                c.Export(typeof(GenericClassA<>)).AsKeyed(typeof(IGenericClass<>), 'A');
                c.Export(typeof(GenericClassB<>)).AsKeyed(typeof(IGenericClass<>), 'B');
            });

            var instance = container.Locate<DependentService<IGenericClass<int>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<GenericClassA<int>>(instance.Value);
        }

        [Fact]
        public void Locate_Open_Generic_Filtered()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).WithCtorParam<object>().Named("value").Consider(s => s.ActivationType.Name.EndsWith("B`1"));
                c.Export(typeof(GenericClassA<>)).As(typeof(IGenericClass<>));
                c.Export(typeof(GenericClassB<>)).As(typeof(IGenericClass<>));
                c.Export(typeof(GenericClassC<>)).As(typeof(IGenericClass<>));
            });

            var instance = container.Locate<DependentService<IGenericClass<int>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<GenericClassB<int>>(instance.Value);
        }
        

        [Fact]
        public void Generic_When_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(GenericClassA<>)).As(typeof(IGenericClass<>)).When.GenericArgumentIs(typeof(BasicService));
                c.Export(typeof(GenericClassB<>)).As(typeof(IGenericClass<>));
            });

            var instance = container.Locate<IGenericClass<BasicService>>();

            Assert.NotNull(instance);
            Assert.IsType<GenericClassA<BasicService>>(instance);

            var instance2 = container.Locate<IGenericClass<IBasicService>>();

            Assert.NotNull(instance2);
            Assert.IsType<GenericClassB<IBasicService>>(instance2);
        }
    }

    public static class WhenExtension
    {
        public static IWhenConditionConfiguration<IFluentExportStrategyConfiguration> GenericArgumentIs(this IWhenConditionConfiguration<IFluentExportStrategyConfiguration> configuration,
            params Type[] args)
        {
            configuration.MeetsCondition((strategy, context) =>
            {
                var activationType = context.ActivationType;

                if (activationType.IsConstructedGenericType)
                {
                    var genericArgs = activationType.GetGenericArguments();

                    if (args.Length > genericArgs.Length)
                    {
                        return false;
                    }

                    var i = 0;

                    foreach (var type in args)
                    {
                        if (type == null)
                        {
                            continue;
                        }

                        if (type != genericArgs[i])
                        {
                            return false;
                        }

                        i++;
                    }

                    return true;
                }

                return false;
            });

            return configuration;
        }
    }

}
