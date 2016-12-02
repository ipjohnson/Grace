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
    public class WhenClassHasTests
    {
        #region non Generic

        [Fact]
        public void WhenClassHas_Non_Runtime_Values()
        {
            var condition = new WhenClassHas(typeof(SomeTestAttribute));

            Assert.False(condition.IsRequestTimeCondition);
            Assert.False(condition.RequiresInjectionContext);
        }

        [Fact]
        public void WhenClassHas_Match_With_No_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>()
                    .As<IAttributedSimpleObject>()
                    .When.ClassHas(typeof(SomeTestAttribute));
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectB>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectA>(attributedInstance.Value);
        }


        [Fact]
        public void WhenClassHas_Match_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>()
                    .As<IAttributedSimpleObject>()
                    .When.ClassHas(typeof(SomeTestAttribute), attribute => ((SomeTestAttribute)attribute).TestValue == 10);
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>().When.ClassHas(typeof(SomeTestAttribute));
                c.Export<AttributedSimpleObjectC>().As<IAttributedSimpleObject>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectC>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance.Value);

            var otherAttributedInstance = container.Locate<OtherAttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(otherAttributedInstance);
            Assert.NotNull(otherAttributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectA>(otherAttributedInstance.Value);
        }

        #endregion

        #region Generic

        [Fact]
        public void WhenClassHasGeneric_Non_Runtime_Values()
        {
            var condition = new WhenClassHas<SomeTestAttribute>();

            Assert.False(condition.IsRequestTimeCondition);
            Assert.False(condition.RequiresInjectionContext);
        }

        [Fact]
        public void WhenClassHasGeneric_Match_With_No_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.ClassHas<SomeTestAttribute>();
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectB>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectA>(attributedInstance.Value);
        }


        [Fact]
        public void WhenClassHasGeneric_Match_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.ClassHas<SomeTestAttribute>(attribute => attribute.TestValue == 10);
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>().When.ClassHas<SomeTestAttribute>();
                c.Export<AttributedSimpleObjectC>().As<IAttributedSimpleObject>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectC>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance.Value);

            var otherAttributedInstance = container.Locate<OtherAttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(otherAttributedInstance);
            Assert.NotNull(otherAttributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectA>(otherAttributedInstance.Value);
        }
        #endregion
    }
}
