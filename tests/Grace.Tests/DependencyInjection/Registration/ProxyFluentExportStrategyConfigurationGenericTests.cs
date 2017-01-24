using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.Attributes;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class ProxyFluentExportStrategyConfigurationGenericTests
    {
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_As([Locate]FluentWithCtorConfiguration<BasicService, int> configuration, 
                                                              IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.As(typeof(IBasicService));

            strategyConfiguration.Received().As(typeof(IBasicService));
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_As_Generic([Locate]FluentWithCtorConfiguration<BasicService, int> configuration, 
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration )
        {
            configuration.As<IBasicService>();

            strategyConfiguration.Received().As<IBasicService>();
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_AsKeyed_Generic(
            [Locate] FluentWithCtorConfiguration<BasicService, int> configuration,
            IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.AsKeyed<IBasicService>('C');

            strategyConfiguration.Received().AsKeyed<IBasicService>('C');
        }
        
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_AsKeyed(
            [Locate] FluentWithCtorConfiguration<BasicService, int> configuration,
            IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.AsKeyed(typeof(IBasicService),'C');

            strategyConfiguration.Received().AsKeyed(typeof(IBasicService), 'C');
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ExternallyOwned([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.ExternallyOwned();

            strategyConfiguration.Received().ExternallyOwned();
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportMethods([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Expression<Action<BasicService>> action = service => service.TestMethod();

            configuration.ImportMethod(action);

            strategyConfiguration.Received().ImportMethod(action);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportMembers([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<MemberInfo, bool> func = info => true;

            configuration.ImportMembers(func);

            strategyConfiguration.Received().ImportMembers(func);
        }
        
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportProperty([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Expression<Func<BasicService, int>> func = service => service.Count;

            configuration.ImportProperty(func);

            strategyConfiguration.Received().ImportProperty(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ExportMember([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Expression<Func<BasicService, int>> func = service => service.Count;

            configuration.ExportMember(func);

            strategyConfiguration.Received().ExportMember(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ActivationMethod([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Expression<Action<BasicService>> func = service => service.TestMethod();

            configuration.ActivationMethod(func);

            strategyConfiguration.Received().ActivationMethod(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_Apply([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Action<BasicService> func = service => service.TestMethod();

            configuration.Apply(func);

            strategyConfiguration.Received().Apply(func);
        }
        
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ByInterfaces([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<Type, bool> func = type => true;

            configuration.ByInterfaces(func);

            strategyConfiguration.Received().ByInterfaces(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_DisposalCleanup([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Action<BasicService> func = service => service.TestMethod();

            configuration.DisposalCleanupDelegate(func);

            strategyConfiguration.Received().DisposalCleanupDelegate(func);
        }
        
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_EnrichWithDelegate([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<IExportLocatorScope, StaticInjectionContext, BasicService, BasicService> enrichmentDelegate = (scope,staticonctext,context) => new BasicService();

            configuration.EnrichWithDelegate(enrichmentDelegate);

            strategyConfiguration.Received().EnrichWithDelegate(enrichmentDelegate);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_UsingLifestyle([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            var lifestyle = new SingletonLifestyle();

            configuration.UsingLifestyle(lifestyle);

            strategyConfiguration.Lifestyle.Received().Custom(lifestyle);
        }
        
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<int> func = () => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam_Multi_1_Arg([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<IExportLocatorScope, int> func = (scope) => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam_Multi_2_Arg([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<IExportLocatorScope, StaticInjectionContext, int> func = (scope, staticContext) => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }


        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam_Multi_3_Arg([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<IExportLocatorScope,StaticInjectionContext,IInjectionContext, int> func = (scope,staticContext,context) => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam_Multi_4_Arg([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext,IBasicService, int> func = (scope, staticContext, context, basic) => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam_Multi_5_Arg([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext,IBasicService,IMultipleService, int> func = (scope, staticContext, context, basic, multiple) => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorCollectionParam([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.WithCtorCollectionParam<IEnumerable<int>,int>();

            strategyConfiguration.Received().WithCtorCollectionParam<IEnumerable<int>,int>();
        }
        
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithMetadata([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.WithMetadata(5, 10);

            strategyConfiguration.Received().WithMetadata(5, 10);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithPriority([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.WithPriority(10);

            strategyConfiguration.Received().WithPriority(10);
        }
    }
}
