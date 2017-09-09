using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class BestMatchTests
    {
        [Fact]
        public void Container_Match_Best_Constructor_No_Arg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<MultipleConstructorImport>());

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.Null(service.BasicService);
            Assert.Null(service.ConstructorImportService);
        }

        [Fact]
        public void Container_Match_Best_Constructor_One_Arg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleConstructorImport>();
                c.Export<BasicService>().As<IBasicService>();
            });

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.NotNull(service.BasicService);
            Assert.Null(service.ConstructorImportService);
        }

        [Fact]
        public void Container_Match_Best_Constructor_Two_Args()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleConstructorImport>();
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
            });

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.NotNull(service.BasicService);
            Assert.NotNull(service.ConstructorImportService);
        }


        public interface IHaveMultipleConstructors
        {
            int NumProp { get; }

            string StringProp { get; }
        }


        public class HaveMultipleConstructors : IHaveMultipleConstructors
        {
            public HaveMultipleConstructors()
            {

            }

            public HaveMultipleConstructors(string stringValue, int intValue)
            {

                NumProp = intValue;

                StringProp = stringValue;
            }

            public int NumProp { get; private set; }

            public string StringProp { get; private set; }
        }

        [Fact]
        public void Container_Match_Best_Constructor_Func()
        {
            DependencyInjectionContainer di = new DependencyInjectionContainer();

            di.Configure(config =>
            {
                config.Export<HaveMultipleConstructors>().As<IHaveMultipleConstructors>();
            });

            var myFunc = di.Locate<Func<string, int, IHaveMultipleConstructors>>();

            var functioned = myFunc("funcString", 667);
        }

        public class MultipleConstructorWithFunc
        {
            public MultipleConstructorWithFunc()
            {
                
            }

            public MultipleConstructorWithFunc(Func<IBasicService> basicService)
            {
                BasicService = basicService;
            }

            public Func<IBasicService> BasicService { get; }
        }

        [Fact]
        public void Container_Best_Match_NoFunc()
        {
            var container = new DependencyInjectionContainer();

            var instance = container.Locate<MultipleConstructorWithFunc>();

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);
        }

        public class ConcreteTestClass
        {
            public ConcreteTestClass()
            {
                throw new Exception("Should not get here");
            }
        }

        public class MultipleConstructorWithConcreteType
        {
            public MultipleConstructorWithConcreteType(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public MultipleConstructorWithConcreteType(IBasicService basicService, ConcreteTestClass concreteTestClass)
            {
                BasicService = basicService;
                ConcreteTestClass = concreteTestClass;
            }

            public IBasicService BasicService { get; }

            public ConcreteTestClass ConcreteTestClass { get; }
        }

        [Fact]
        public void BestMatchConstructor_FilterConcreteType()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExcludeTypeFromAutoRegistration("*ConcreteTestClass");
            });

            var instance = container.Locate<MultipleConstructorWithConcreteType>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Null(instance.ConcreteTestClass);
        }
    }
}
