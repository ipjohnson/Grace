using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection
{
    public class BasicContainerTests
    {
        [Fact]
        public void DependencyInjectionContainer_Create()
        {
            var container = new DependencyInjectionContainer { c => c.Export<BasicService>().As<IBasicService>() };
            
            var basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.IsType<BasicService>(basicService);
        }

        public class TestModule : IConfigurationModule
        {
            public void Configure(IExportRegistrationBlock registrationBlock)
            {
                registrationBlock.Export<BasicService>().As<IBasicService>();
            }
        }

        [Fact]
        public void DependencyInjectionContainer_Add_Module()
        {
            var container = new DependencyInjectionContainer { new TestModule() };

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void DependencyInjectionContainer_Configure_Module()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(new TestModule());

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void DependencyInjectionContainer_Configure_Null_Module_Throws()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ArgumentNullException>(() => container.Configure((IConfigurationModule) null));
        }
        
        [Fact]
        public void DependencyInjectionContainer_Add_Module_Throws_Exception_For_Null()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ArgumentNullException>(() => container.Add((IConfigurationModule)null));
        }

        [Fact]
        public void DependencyInjectionContainer_Add_Registration_Throws_Exception_For_Null()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ArgumentNullException>(() => container.Add((Action<IExportRegistrationBlock>)null));
        }
        
        [Fact]
        public void DependencyInjectionContainer_Enumerable_Return_Empty()
        {
            var container = new DependencyInjectionContainer { new TestModule() };

            Assert.Empty((IEnumerable<object>)container);
            Assert.Empty((IEnumerable)container);
        }

        public class CustomConfiguration : InjectionScopeConfiguration
        {
            
        }

        [Fact]
        public void DependencyInjectionContainer_Create_Configuration()
        {
            var container = new DependencyInjectionContainer(new CustomConfiguration()) { new TestModule() };

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        public interface IInheritingBasicService : IBasicService
        {
            
        }

        public class InheritingBasicService : BasicService, IInheritingBasicService
        {
            
        }

        [Fact]
        public void DepencencyInjectionContainer_ExportAsBase_Interface()
        {
            var container = new DependencyInjectionContainer(c => c.ExportAsBase = true);

            container.Configure(c => c.Export<InheritingBasicService>().As<IInheritingBasicService>().ImportProperty(m => m.Count).DefaultValue(5));

            var instance = container.Locate<IInheritingBasicService>();
            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);

            var basicInstance = container.Locate<IBasicService>();
            Assert.NotNull(basicInstance);
            Assert.Equal(5, basicInstance.Count);
        }


        [Fact]
        public void DepencencyInjectionContainer_ExportAsBase_Concrete()
        {
            var container = new DependencyInjectionContainer(c => c.ExportAsBase = true);

            container.Configure(c => c.Export<InheritingBasicService>().ImportProperty(m => m.Count).DefaultValue(5));

            var instance = container.Locate<InheritingBasicService>();
            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);

            var basicInstance = container.Locate<BasicService>();
            Assert.NotNull(basicInstance);
            Assert.Equal(5, basicInstance.Count);
        }

        [Fact]
        public void DependencyInjectionContainer_Keys()
        {
            var container = new DependencyInjectionContainer();

            container.SetExtraData(5, 10);

            var keys = container.Keys.ToArray();

            Assert.Equal(1, keys.Length);
        }
        
        [Fact]
        public void DependencyInjectionContainer_Values()
        {
            var container = new DependencyInjectionContainer();

            container.SetExtraData(5, 10);

            var values = container.Values.ToArray();

            Assert.Equal(1, values.Length);
        }
        
        [Fact]
        public void DependencyInjectionContainer_KeyValuePairs()
        {
            var container = new DependencyInjectionContainer();

            container.SetExtraData(5, 10);

            var values = container.KeyValuePairs.ToArray();

            Assert.Equal(1, values.Length);
        }


        [Fact]
        public void DependencyInjectionContainer_Clone_Strategies()
        {
            // turn off func support so no strategies
            var container = new DependencyInjectionContainer(c => c.SupportFuncType = false);

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var collection = container.StrategyCollectionContainer.Clone();

            Assert.NotNull(collection);
            Assert.Equal(1, collection.GetAllStrategies().Count());
        }

        public class ImportInjectionScope
        {
            public ImportInjectionScope(IInjectionScope injectionScope)
            {
                InjectionScope = injectionScope;
            }

            public IInjectionScope InjectionScope { get; }
        }

        [Fact]
        public void DependencyInjectionContainer_Import_InjectionScope()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.AllowInjectionScopeLocation = true);

            var instance = container.Locate<ImportInjectionScope>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.InjectionScope);
            Assert.Same(container, instance.InjectionScope);
        }

        [Fact]
        public void DependencyInjectionContainer_Locate_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
            });

            var instance = container.Locate<IMultipleService>(consider: e => e.ActivationType.Name.EndsWith("2"));

            Assert.NotNull(instance);
            Assert.IsType<MultipleService2>(instance);
        }
    }
}
