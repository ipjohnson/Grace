using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class ExportStrategyProviderAttributeTest
    {
        [Fact]
        public void ExportMagicAttributeTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(
                block => block.ExportAssemblyContaining<ExportStrategyProviderAttributeTest>()
                    .Where(TypesThat.AreInTheSameNamespaceAs<ExportStrategyProviderAttributeTest>())
                    .ExportAttributedTypes()
                );

            var depService = container.Locate<IDependentService<SomeWrappedTyped>>();

            Assert.NotNull(depService);
        }
    }

    public class MagicExportAttribute : Attribute, IExportStrategyProviderAttribute
    {
        public ICompiledExportStrategy ProvideStrategy(Type attributedType, IActivationStrategyCreator creator)
        {
            var newType = typeof(DependentService<>).MakeGenericType(attributedType);
            var exportType = typeof(IDependentService<>).MakeGenericType(attributedType);

            var strategy = creator.GetCompiledExportStrategy(newType);

            strategy.AddExportAs(exportType);

            return strategy;
        }
    }

    [MagicExport]
    public class SomeWrappedTyped
    {

    }
}
