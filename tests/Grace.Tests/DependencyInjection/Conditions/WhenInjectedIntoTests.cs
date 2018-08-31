using System;
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
        public void WhenInjectedInto_Null_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new WhenInjectedInto((Type[])null));
            Assert.Throws<ArgumentNullException>(() => new WhenInjectedInto((Func<Type, bool>)null));
        }

        [Fact]
        public void WhenInjectedIntoGeneric_Matches_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.InjectedInto<DependentService<IAttributedSimpleObject>>();
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>().When.InjectedInto<AttributedDependentService<IAttributedSimpleObject>>();
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

        [Fact]
        public void WhenInjectedInto_Not_Used_For_Locate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.InjectedInto(typeof(DependentService<>));
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectA>(instance.Value);

            var attributedInstance = container.Locate<IAttributedSimpleObject>();

            Assert.NotNull(attributedInstance);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance);
        }

        public class SimpleObjectDecorator : IAttributedSimpleObject
        {
            private IAttributedSimpleObject _decorated;

            public SimpleObjectDecorator(IAttributedSimpleObject decorated)
            {
                _decorated = decorated;
            }
        }

        [Fact]
        public void WhenInjectedInto_Wrapped()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>();
                c.ExportDecorator(typeof(SimpleObjectDecorator)).As(typeof(IAttributedSimpleObject)).When
                    .InjectedInto(typeof(DependentService<>));
            });

            var instanceFunc = container.Locate<DependentService<Func<IAttributedSimpleObject>>>();

            Assert.NotNull(instanceFunc);

            var instance = instanceFunc.Value();

            Assert.NotNull(instance);
            Assert.IsType<SimpleObjectDecorator>(instance);
        }
    }
}
