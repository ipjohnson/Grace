using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.MemberInjection
{
    public class MethodInjectionTests
    {
        [Fact]
        public void MethodInjection_OneArg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ImportMethod(m => m.InjectMethod(Arg.Any<IBasicService>()));
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);
        }

        [Fact]
        public void MethodInjection_Inject_Members_That_Are_Methods()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ImportMembers(MembersThat.AreMethod(), injectMethod: true);
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);

            Assert.NotNull(instance.SecondService);
            Assert.IsType<BasicService>(instance.SecondService);
        }

        [Fact]
        public void MethodInjection_Inject_Members_That_Are_Method_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ImportMembers(MembersThat.AreMethod(m => m.Name.StartsWith("Some")), true);
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);

            Assert.NotNull(instance.SecondService);
            Assert.IsType<BasicService>(instance.SecondService);
        }

        [Fact]
        public void MethodInjection_Inject_Int_To_Method()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance(5).AsKeyed<int>("value");
                c.Export<ImportIntMethodClass>().ImportMethod(m => m.SetValue(Arg.Any<int>()));
            });

            var instance = container.Locate<ImportIntMethodClass>();

            Assert.NotNull(instance);

            Assert.Equal(5, instance.Value);
        }

        [Fact]
        public void MethodInjection_Specify_Parameter_Info()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.AddInspector(new MethodInjectionSpecifyParameterInfoSelector());
                c.Export<BasicService>().AsKeyed<IBasicService>("Hello");
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.BasicService);
            Assert.Null(instance.SecondService);
        }

        [Fact]
        public void MethodInjection_Specify_Parameter_Info_Without_Service()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.AddInspector(new MethodInjectionSpecifyParameterInfoSelector(true));
                c.Export<BasicService>().AsKeyed<IBasicService>("Hello");
            });

            Assert.Throws<LocateException>(() => container.Locate<MethodInjectionClass>());
        }

        private class MethodInjectionSpecifyParameterInfoSelector : IActivationStrategyInspector, IMemberInjectionSelector
        {
            private bool vooDooIsRequired;

            public MethodInjectionSpecifyParameterInfoSelector(bool vooDooIsRequired = false)
            {
                this.vooDooIsRequired = vooDooIsRequired;
            }

            public IEnumerable<MethodInjectionInfo> GetMethods(
                Type type, 
                IInjectionScope injectionScope, 
                IActivationExpressionRequest request)
            {
                if (type != typeof(MethodInjectionClass))
                    return ImmutableLinkedList<MethodInjectionInfo>.Empty;

                var methods = new MethodInjectionInfo[2];

                var injectMethod = type.GetMethod(nameof(MethodInjectionClass.InjectMethod));
                methods[0] = new MethodInjectionInfo() { Method = injectMethod };
                methods[0].MethodParameterInfo(new MethodParameterInfo("basicService")
                {
                    LocateKey = "Hello",
                    IsRequired = true,
                });

                var someOtherMethod = type.GetMethod(nameof(MethodInjectionClass.SomeOtherMethod));
                methods[1] = new MethodInjectionInfo() { Method = someOtherMethod };
                methods[1].MethodParameterInfo(new MethodParameterInfo("basicService")
                {
                    LocateKey = "VooDoo",
                    IsRequired = vooDooIsRequired,
                });

                return methods;
            }

            public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, 
            IInjectionScope injectionScope, 
            IActivationExpressionRequest request)
            {
                return ImmutableLinkedList<MemberInjectionInfo>.Empty;
            }

            void IActivationStrategyInspector.Inspect<T>(T strategy)
            {
                if (strategy.ActivationType == typeof(MethodInjectionClass))
                    strategy.GetActivationConfiguration(strategy.ActivationType).MemberInjectionSelector(this);
            }
        }
    }
}
