using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class ChildContainerConstructorExpressionCreatorTests
    {
        [Fact]
        public void ChildContainerLocator()
        {
            var container = new DependencyInjectionContainer(c =>
                c.Behaviors.ConstructorSelection = new ChildContainerConstructorExpressionCreator());

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)));

            var childContainer = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>());

            var instance = childContainer.Locate<IDependentService<IBasicService>>();
        }

        [Fact]
        public void ChildContainerLocateCantFind()
        {
            var container = new DependencyInjectionContainer(c =>
                c.Behaviors.ConstructorSelection = new ChildContainerConstructorExpressionCreator());

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)));

            var childContainer = container.CreateChildScope();

            Assert.Throws<LocateException>(() => childContainer.Locate<IDependentService<IBasicService>>());
        }
    }
}
