using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Diagnostics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    public class InjectionScopeDiagnosticsTests
    {
        [Fact]
        public void InjectionScopeDiagnostics_Check_For_Missing_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DependentService<IBasicService>>();
            });

            var diagnostic = container.Locate<InjectionScopeDiagnostics>();

            Assert.NotNull(diagnostic);

            var missingDependencies = diagnostic.PossibleMissingDependencies.ToArray();

            Assert.Equal(1, missingDependencies.Length);

            var dependency = missingDependencies[0];

            Assert.Equal("value", dependency.MemberName);
            Assert.Equal(DependencyType.ConstructorParameter, dependency.DependencyType);
            Assert.Equal(typeof(IBasicService), dependency.TypeBeingImported);
        }
    }
}
