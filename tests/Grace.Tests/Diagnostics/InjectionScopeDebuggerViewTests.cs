using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Diagnostics;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    public class InjectionScopeDebuggerViewTests
    {
        [Fact]
        public void InjectionScopeDebuggerView_Basic_Tests()
        {
            var container = new DependencyInjectionContainer();

            var debugger = new InjectionScopeDebuggerView(container);

            Assert.Equal(container.ScopeId, debugger.ScopeId);
            Assert.Equal(container.ScopeName, debugger.Name);
            Assert.Equal(container.ScopeConfiguration, debugger.Configuration);
            Assert.NotNull(debugger.ExtraData);
            Assert.NotNull(debugger.Decorators);
            Assert.NotNull(debugger.Exports);
            Assert.NotNull(debugger.ExportByType);
            Assert.NotNull(debugger.Wrappers);
            Assert.Empty(debugger.PossibleMissingDependencies.Items);
            Assert.Empty(debugger.ContainerExceptions.Items);
        }
    }
}
