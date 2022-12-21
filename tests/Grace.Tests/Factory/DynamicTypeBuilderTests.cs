using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Factory.Impl;
using Grace.Tests.Classes.Simple;
using Grace.Utilities;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.Factory
{
    [SubFixtureInitialize]
    public class DynamicTypeBuilderTests
    {
        public interface IBasicServiceProvider
        {
            IBasicService CreateBasicService();
        }

        [Theory]
        [AutoData]
        public void DynamicTypeBuilder_NoParams(DynamicTypeBuilder builder, IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context)
        {
            var proxyType = builder.CreateType(typeof(IBasicServiceProvider), out List<DynamicTypeBuilder.DelegateInfo> methods);

            var newBasicService = new BasicService();

            var func = new ActivationStrategyDelegate((s, d, c) => newBasicService);

            var instance = (IBasicServiceProvider)Activator.CreateInstance(proxyType, scope, disposalScope, context, func);

            var basicService = instance.CreateBasicService();

            Assert.NotNull(basicService);
            Assert.Same(newBasicService, basicService);
        }

        public interface IComplexProvider<T>
        {
            IDependentService<T> CreateValue(T i);
        }


        [Theory]
        [AutoData]
        public void DynamicTypeBuilder_IntParam(DynamicTypeBuilder builder, IExportLocatorScope scope, IDisposalScope disposalScope, InjectionContext context)
        {
            var proxyType = builder.CreateType(typeof(IComplexProvider<int>), out List<DynamicTypeBuilder.DelegateInfo> methods);

            int value = 10;
            DependentService<int> service = null;

            var func = new ActivationStrategyDelegate((s, d, c) =>
            {
                Assert.Contains(c.Keys, key => key.ToString().StartsWith(UniqueStringId.Prefix) &&
                                              Equals(c.GetExtraData(key), value));
                return service = new DependentService<int>(value);
            });

            var instance = (IComplexProvider<int>)Activator.CreateInstance(proxyType, scope, disposalScope, context, func);

            var instanceService = instance.CreateValue(value);

            Assert.NotNull(instanceService);
            Assert.Same(service, instanceService);
        }

        [Theory]
        [AutoData]
        public void DynamicTypeBuilder_StringParam(DynamicTypeBuilder builder, IExportLocatorScope scope, IDisposalScope disposalScope, InjectionContext context)
        {
            var proxyType = builder.CreateType(typeof(IComplexProvider<string>), out List<DynamicTypeBuilder.DelegateInfo> methods);

            string value = "hello world";
            DependentService<string> service = null;

            var func = new ActivationStrategyDelegate((s, d, c) =>
            {
                Assert.Contains(c.Keys, key => key.ToString().StartsWith(UniqueStringId.Prefix) &&
                                              Equals(c.GetExtraData(key), value));
                return service = new DependentService<string>(value);
            });

            var instance = (IComplexProvider<string>)Activator.CreateInstance(proxyType, scope, disposalScope, context, func);

            var instanceService = instance.CreateValue(value);

            Assert.NotNull(instanceService);
            Assert.Same(service, instanceService);
        }
    }
}
