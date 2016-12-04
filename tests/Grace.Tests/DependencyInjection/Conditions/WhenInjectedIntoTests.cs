using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Conditions
{
    public class WhenInjectedIntoTests
    {
        
        [Fact]
        public void WhenInjectedInto_Matches_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.InjectedInto(typeof(DependentService<>));
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>().When.InjectedInto(typeof(AttributedDependentService<>));
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectA>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance.Value);
        }
    }
}
