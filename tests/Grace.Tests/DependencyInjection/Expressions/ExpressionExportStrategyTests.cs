using System;
using System.Collections.Generic;
using System.Text;
using Grace.Data;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Expressions
{
    public class ExpressionExportStrategyTests
    {
        public class TwoArgClass
        {
            public TwoArgClass(IBasicService basicService, int intValue)
            {
                BasicService = basicService;
                IntValue = intValue;
            }

            public IBasicService BasicService { get; }

            public int IntValue { get; }
        }

        [Fact]
        public void ExpressionExport_LocateValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportExpression(() => new TwoArgClass(Arg.Locate<IBasicService>(), 5));
            });

            var instance = container.Locate<TwoArgClass>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.IntValue);
            Assert.IsType<BasicService>(instance.BasicService);
        }

        public class ScopeNameClass
        {
            public ScopeNameClass(IBasicService basicService, string scopeName)
            {
                BasicService = basicService;
                ScopeName = scopeName;
            }

            public IBasicService BasicService { get; }

            public string ScopeName { get; }
        }

        [Fact]
        public void ExpressionExport_ScopeValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportExpression(() => new ScopeNameClass(Arg.Locate<IBasicService>(), Arg.Scope().ScopeName));
            });

            var instance = container.Locate<ScopeNameClass>();

            Assert.NotNull(instance);
            Assert.Equal(container.ScopeName, instance.ScopeName);

            var scopeName = "SomeScope";

            using (var scope = container.BeginLifetimeScope(scopeName))
            {
                instance = scope.Locate<ScopeNameClass>();

                Assert.NotNull(instance);
                Assert.Equal(scopeName, instance.ScopeName);
            }
        }

        public class InjectionContextClass
        {
            public InjectionContextClass(IBasicService basicService, int someValue)
            {
                BasicService = basicService;
                SomeValue = someValue;
            }

            public IBasicService BasicService { get; }

            public int SomeValue { get; }
        }

        [Fact]
        public void ExpressionExport_InjectionContext()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportExpression(() =>
                    new InjectionContextClass(Arg.Locate<IBasicService>(), Arg.Context().GetExtraData<int>("somevalue")));
            });

            var instance = container.Locate<InjectionContextClass>(new { SomeValue = 10 });

            Assert.NotNull(instance);
            Assert.Equal(10, instance.SomeValue);
        }

        public class IntProviderClass
        {
            public int SomeValue { get; } = 20;
        }

        [Fact]
        public void ExpressionExport_LocateWithData()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportExpression(() =>
                    new DependentService<InjectionContextClass>(Arg.Locate<InjectionContextClass>(new { Arg.Locate<IntProviderClass>().SomeValue })));
            });

            var instance = container.Locate<DependentService<InjectionContextClass>>();

            Assert.NotNull(instance);
            Assert.Equal(20, instance.Value.SomeValue);
        }
    }
}

