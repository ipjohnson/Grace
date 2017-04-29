using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Locate
{
    [SubFixtureInitialize]
    public class CanLocateTypeServiceTests
    {
        [Theory]
        [AutoData]
        public void CanLocateTypeService_CanLocate_Array(CanLocateTypeService service, IInjectionScope scope)
        {
            Assert.True(service.CanLocate(scope, typeof(IBasicService[]), null));
        }
        
        [Theory]
        [AutoData]
        public void CanLocateTypeService_CanLocate_Concrete(CanLocateTypeService service, IInjectionScope scope)
        {
            Assert.True(service.CanLocate(scope, typeof(BasicService), null));
        }

        [Theory]
        [AutoData]
        public void CanLocateTypeService_CanLocate_Wrapper(CanLocateTypeService service, IInjectionScope scope, IActivationStrategyCollection<ICompiledWrapperStrategy> collection)
        {
            scope.WrapperCollectionContainer.
                GetActivationStrategyCollection(typeof(Func<IBasicService>)).Returns(collection);

            Assert.True(service.CanLocate(scope, typeof(Func<IBasicService>), null));
        }
        
        [Theory]
        [AutoData]
        public void CanLocateTypeService_CanLocate_Strategy_Generic(CanLocateTypeService service, IInjectionScope scope, IActivationStrategyCollection<ICompiledExportStrategy> collection)
        {
            scope.StrategyCollectionContainer.
                GetActivationStrategyCollection(typeof(DependentService<>)).Returns(collection);

            Assert.True(service.CanLocate(scope, typeof(DependentService<IBasicService>), null));
        }

        [Theory]
        [AutoData]
        public void CanLocateTypeService_CanLocate_Wrapper_Generic(CanLocateTypeService service, IInjectionScope scope, IActivationStrategyCollection<ICompiledWrapperStrategy> collection)
        {
            scope.WrapperCollectionContainer.
                GetActivationStrategyCollection(typeof(Func<>)).Returns(collection);

            Assert.True(service.CanLocate(scope, typeof(Func<IBasicService>), null));
        }
    }
}
