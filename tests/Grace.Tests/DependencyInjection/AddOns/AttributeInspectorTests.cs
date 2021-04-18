using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class AttributeInspectorTests
    {
        [Fact]
        public void AttributeInspector_ProcessAttributesForStrategy()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.AddInspector(new FluentAttributeProcessor());
                c.Export<Child>().As<IChild>();
                c.Export<Parent>().As<IParent>();
            });

            var parent = container.Locate<IParent>();

            Assert.NotNull(parent);
            Assert.NotNull(parent.ChildInstance);

        }

        #region inspector
        public class FluentAttributeProcessor : IActivationStrategyInspector
        {
            public void Inspect<T>(T strategy) where T : class, IActivationStrategy
            {
                if (strategy is ICompiledExportStrategy exportStrategy)
                {
                    exportStrategy.ProcessAttributeForStrategy();
                }
            }
        }
        #endregion

        #region Test Classes
        public interface IChild { }
        public class Child : IChild { }

        public interface IParent
        {
            IChild ChildInstance { get; }
        }

        public class Parent : IParent
        {
            [Import] public IChild ChildInstance { get; set; }
        }
        #endregion
    }
}
