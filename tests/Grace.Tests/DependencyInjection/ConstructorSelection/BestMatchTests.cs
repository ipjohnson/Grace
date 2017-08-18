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
        
    }
}
