using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Misc
{
    public class CompositePatternTests
    {
        public class TestContext
        {
            public Guid Id { get; set; } = Guid.Empty;
        }

        [Fact]
        public void CompositeFactoryTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<TestContext>();
                c.ExportFactory<TestContext, TestContext>(context =>
                {
                    context.Id = Guid.NewGuid();
                    return context;
                });
            });

            var instance = container.Locate<TestContext>();

            Assert.NotNull(instance);
            Assert.NotEqual(instance.Id, Guid.Empty);
        }


        [Fact]
        public void CompositeFactoryFuncTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<TestContext>();
                c.ExportFactory<Func<TestContext>, TestContext>(func =>
                {
                    var context = func();
                    context.Id = Guid.NewGuid();
                    return context;
                });
            });

            var instance = container.Locate<TestContext>();

            Assert.NotNull(instance);
            Assert.NotEqual(instance.Id, Guid.Empty);
        }


    }
}
