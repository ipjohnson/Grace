using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Attributes;
using Xunit;

namespace Grace.Tests.DependencyInjection.Inject
{
    public class InjectExtensionTests
    {
        [Fact]
        public void Inject_Property()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<AttributeBasicService>().As<IAttributeBasicService>());

            var instance = new AttributedImportPropertyService();

            container.Inject(instance);

            Assert.NotNull(instance.BasicService);
        }

        [Fact]
        public void Inject_Property_From_LifetimeScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<AttributeBasicService>().As<IAttributeBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = new AttributedImportPropertyService();

                scope.Inject(instance);

                Assert.NotNull(instance.BasicService);
            }
        }

        [Fact]
        public void Inject_Method()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<AttributeBasicService>().As<IAttributeBasicService>());

            var instance = new AttributedImportMethodService();

            container.Inject(instance);

            Assert.NotNull(instance.BasicService);
        }


        public class TestClass
        {
            [Import]
            public int PropA { get; set; }
        }

        [Fact]
        public void Inject_With_ExtraData()
        {
            var container = new DependencyInjectionContainer();

            var testClass = new TestClass();

            container.Inject(testClass, new { PropA = 5 });

            Assert.Equal(5, testClass.PropA);
        }
    }
}
