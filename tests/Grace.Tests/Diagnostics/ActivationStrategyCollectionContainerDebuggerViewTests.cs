using System.Reflection;
using Grace.DependencyInjection;
using Grace.Diagnostics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    public class ActivationStrategyCollectionContainerDebuggerViewTests
    {
        [Fact]
        public void ActivationStrategyCollectionContainerDebuggerView_Types()
        {
            var debugger =
                new ActivationStrategyCollectionContainerDebuggerView<ICompiledExportStrategy>(
                    ConfigureContainer().StrategyCollectionContainer);

            Assert.Equal(2, debugger.StrategiesByType.Length);
            Assert.Equal(typeof(IBasicService).FullName, debugger.StrategiesByType[0].TypeName);
            Assert.Equal(typeof(IBasicService), debugger.StrategiesByType[0].Type);
            Assert.Single(debugger.StrategiesByType[0].Items);
        }

        [Fact]
        public void ActivationStrategyCollectionContainerDebuggerView_DebuggerDisplayValue()
        {
            var debugger =
                new ActivationStrategyCollectionContainerDebuggerView<ICompiledExportStrategy>(
                    ConfigureContainer().StrategyCollectionContainer);

            var instance = debugger.StrategiesByType[0];

            var property = instance.GetType()
                .GetTypeInfo().GetProperty("DebuggerDisplayValue", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.Equal("Count: 1", property.GetValue(instance));
        }

        [Fact]
        public void ActivationStrategyCollectionContainerDebuggerView_DebuggerDisplayName()
        {
            var debugger =
                new ActivationStrategyCollectionContainerDebuggerView<ICompiledExportStrategy>(
                    ConfigureContainer().StrategyCollectionContainer);

            var instance = debugger.StrategiesByType[0];

            var property = instance.GetType()
                .GetTypeInfo().GetProperty("DebuggerDisplayName", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.Equal(typeof(IBasicService).FullName, property.GetValue(instance));
        }
        private DependencyInjectionContainer ConfigureContainer()
        {
            var container = new DependencyInjectionContainer(c => c.SupportFuncType = false);

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
            });


            return container;
        }
    }
}
