using System;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class StrategyInspectorTests
    {
        public class StrategyInspectorInjectProperty : IActivationStrategyInspector
        {
            public void Inspect<T>(T strategy) where T : class, IActivationStrategy
            {
                if (strategy is ICompiledExportStrategy exportStrategy)
                {
                    foreach (var propertyInfo in strategy.ActivationType.GetProperties())
                    {
                        if (propertyInfo.CanWrite &&
                            propertyInfo.SetMethod.IsPublic &&
                           !propertyInfo.SetMethod.IsStatic)
                        {
                            var injectionInfo = new MemberInjectionInfo { MemberInfo = propertyInfo };

                            exportStrategy.MemberInjectionSelector(new KnownMemberInjectionSelector(injectionInfo));
                        }
                    }
                }
            }
        }

        [Fact]
        public void StrategyInspectorInjection_Tests()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.AddInspector(new StrategyInspectorInjectProperty());
                c.Export<BasicService>().As<IBasicService>();
                c.Export<PropertyInjectionService>().As<IPropertyInjectionService>();
            });

            var instance = container.Locate<IPropertyInjectionService>(new { Count = 5 });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
        }

        class AnAttributeAttribute : Attribute
        {

        }
        class OneClass
        {
            public OneDependency Dep { get; }

            public OneClass([AnAttribute]OneDependency dep)
            {
                Dep = dep;
            }
        };

        class OneDependency
        {
            public string Value { get; set; } = "SAD FACE";
        }

        class AnInspector : IActivationStrategyInspector
        {
            public void Inspect<T>(T strategy) where T : class, IActivationStrategy
            {
                //We enrich the activation of a certain type
                if (strategy.ActivationType == typeof(OneDependency) && strategy is IConfigurableActivationStrategy conf)
                    conf.EnrichmentDelegate((Func<OneDependency, IExportLocatorScope, IInjectionContext, OneDependency>)DoSomethingToEnrich);
            }

            OneDependency DoSomethingToEnrich(OneDependency instance, IExportLocatorScope scope, IInjectionContext injContext)
            {
                var staticContext = (StaticInjectionContext)injContext.GetExtraData("staticcontext");

                //There I would want to somehow know that we are modifying an instance being injected into a constructor parameter
                //decorated with [AnAttribute], but obviously ctx does not have any information about the original request
                if (staticContext.TargetInfo.InjectionTargetAttributes.OfType<AnAttributeAttribute>().Any())
                    instance.Value = "Success";
                return instance;
            }
        }

        [Fact]
        public void ScopeInspectorTest()
        {
            const string scopeName = "A SCOPE";
            DependencyInjectionContainer container = new DependencyInjectionContainer(c => c.SingletonPerScopeShareContext = true);

            container.Configure(cfg =>
            {
                cfg.ExportFactory((IExportLocatorScope sc, StaticInjectionContext ctx) =>
                {
                    //There would be some magic to get the key name...
                    return sc.Locate<OneDependency>(withKey: sc.ScopeName, extraData: new { StaticContext = ctx }); //Is there any way to Locate while retaining previous StaticInjectionContext? (ctx in this case)
                }).As<OneDependency>();

                //An export for a certain scopeName, keyed to avoid exceptions when Locate is called from a scope not named "A SCOPE"
                cfg.Export<OneDependency>().AsKeyed<OneDependency>(scopeName).Lifestyle.SingletonPerNamedScope(scopeName);

                cfg.AddInspector(new AnInspector());
            });

            using (var scope = container.BeginLifetimeScope(scopeName))
            {
                var injected = scope.Locate<OneClass>();

                Assert.Equal("Success", injected.Dep.Value);
            }
        }


        public class ConcreteInspector : IActivationStrategyInspector
        {
            /// <summary>
            /// Inspect the activation strategy
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="strategy"></param>
            public void Inspect<T>(T strategy) where T : class, IActivationStrategy
            {
                if (strategy is ICompiledExportStrategy compiledExportStrategy)
                {
                    compiledExportStrategy.Lifestyle = new SingletonLifestyle();
                }
            }
        }

        [Fact]
        public void InspectConcreteType()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddInspector(new ConcreteInspector()));

            var instance = container.Locate<BasicService>();
            var instance2 = container.Locate<BasicService>();

            Assert.Same(instance, instance2);
        }

    }
}

