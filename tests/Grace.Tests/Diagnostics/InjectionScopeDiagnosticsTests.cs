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
        
        [Fact]
        public void InjectionScopeDiagnostics_ImportMembers_Check_For_Missing_Method_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MethodInjectionClass>().ImportMembers(MembersThat.AreMethod(m => m.Name.StartsWith("Some")));
            });

            var diagnostic = container.Locate<InjectionScopeDiagnostics>();

            Assert.NotNull(diagnostic);

            var missingDependencies = diagnostic.PossibleMissingDependencies.ToArray();

            Assert.Equal(1, missingDependencies.Length);

            var dependency = missingDependencies[0];

            Assert.Equal("basicService", dependency.MemberName);
            Assert.Equal(DependencyType.MethodParameter, dependency.DependencyType);
            Assert.Equal(typeof(IBasicService), dependency.TypeBeingImported);
        }

        [Fact]
        public void InjectionScopeDiagnostics_ImportMethod_Check_For_Missing_Method_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MethodInjectionClass>().ImportMethod(m => m.InjectMethod(Arg.Any<IBasicService>()));
            });

            var diagnostic = container.Locate<InjectionScopeDiagnostics>();

            Assert.NotNull(diagnostic);

            var missingDependencies = diagnostic.PossibleMissingDependencies.ToArray();

            Assert.Equal(1, missingDependencies.Length);

            var dependency = missingDependencies[0];

            Assert.Equal("basicService", dependency.MemberName);
            Assert.Equal(DependencyType.MethodParameter, dependency.DependencyType);
            Assert.Equal(typeof(IBasicService), dependency.TypeBeingImported);
        }


        [Fact]
        public void InjectionScopeDiagnostics_ActvationMethod_Check_For_Missing_Method_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MethodInjectionClass>().ActivationMethod(m => m.InjectMethod(Arg.Any<IBasicService>()));
            });

            var diagnostic = container.Locate<InjectionScopeDiagnostics>();

            Assert.NotNull(diagnostic);

            var missingDependencies = diagnostic.PossibleMissingDependencies.ToArray();

            Assert.Equal(1, missingDependencies.Length);

            var dependency = missingDependencies[0];

            Assert.Equal("basicService", dependency.MemberName);
            Assert.Equal(DependencyType.MethodParameter, dependency.DependencyType);
            Assert.Equal(typeof(IBasicService), dependency.TypeBeingImported);
        }

        [Fact]
        public void InjectionScopeDiagnostics_ImportMethod_With_Keyed_Value_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<ImportIntMethodClass>().ImportMethod(m => m.SetValue(Arg.Any<int>())));

            var diagnostic = container.Locate<InjectionScopeDiagnostics>();

            Assert.NotNull(diagnostic);

            var missingDependencies = diagnostic.PossibleMissingDependencies.ToArray();

            Assert.Equal(1, missingDependencies.Length);

            var dependency = missingDependencies[0];

            Assert.Equal("value", dependency.MemberName);
            Assert.Equal(DependencyType.MethodParameter, dependency.DependencyType);
            Assert.Equal(typeof(int), dependency.TypeBeingImported);
        }
    }
}
