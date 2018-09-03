using System;
using System.Linq;
using Grace.Data;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Conditions;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Conditions
{
    public class WhenInjectedIntoTests
    {
        [Fact]
        public void WhenInjectedInto_Null_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new WhenInjectedInto((Type[])null));
            Assert.Throws<ArgumentNullException>(() => new WhenInjectedInto((Func<Type, bool>)null));
        }

        [Fact]
        public void WhenInjectedIntoGeneric_Matches_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.InjectedInto<DependentService<IAttributedSimpleObject>>();
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>().When.InjectedInto<AttributedDependentService<IAttributedSimpleObject>>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectA>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance.Value);
        }

        [Fact]
        public void WhenInjectedInto_Matches_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.InjectedInto(typeof(DependentService<>));
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>().When.InjectedInto(typeof(AttributedDependentService<>));
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectA>(instance.Value);

            var attributedInstance = container.Locate<AttributedDependentService<IAttributedSimpleObject>>();

            Assert.NotNull(attributedInstance);
            Assert.NotNull(attributedInstance.Value);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance.Value);
        }

        [Fact]
        public void WhenInjectedInto_Not_Used_For_Locate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>().When.InjectedInto(typeof(DependentService<>));
                c.Export<AttributedSimpleObjectB>().As<IAttributedSimpleObject>();
            });

            var instance = container.Locate<DependentService<IAttributedSimpleObject>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<AttributedSimpleObjectA>(instance.Value);

            var attributedInstance = container.Locate<IAttributedSimpleObject>();

            Assert.NotNull(attributedInstance);
            Assert.IsType<AttributedSimpleObjectB>(attributedInstance);
        }

        public class SimpleObjectDecorator : IAttributedSimpleObject
        {
            private IAttributedSimpleObject _decorated;

            public SimpleObjectDecorator(IAttributedSimpleObject decorated)
            {
                _decorated = decorated;
            }
        }

        [Fact]
        public void WhenInjectedInto_Wrapped()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<AttributedSimpleObjectA>().As<IAttributedSimpleObject>();
                c.ExportDecorator(typeof(SimpleObjectDecorator)).As(typeof(IAttributedSimpleObject)).When
                    .InjectedInto(typeof(DependentService<>));
            });

            var instanceFunc = container.Locate<DependentService<Func<IAttributedSimpleObject>>>();

            Assert.NotNull(instanceFunc);

            var instance = instanceFunc.Value();

            Assert.NotNull(instance);
            Assert.IsType<SimpleObjectDecorator>(instance);
        }


        #region Complex condition test for issue #192

        public interface IMyTestService
        {
            Func<IBasicService> ServiceAccessor { get; }
        }

        public class BasicDecorator : IBasicService
        {
            private IBasicService _basicService;

            public BasicDecorator(IBasicService basicService)
            {
                _basicService = basicService;
            }

            public int Count
            {
                get => _basicService.Count;
                set => _basicService.Count = value;
            }

            public int TestMethod()
            {
                return _basicService.TestMethod();
            }
        }

        public class MyTestService : IMyTestService
        {
            [Import]
            public Func<IBasicService> ServiceAccessor { get; set; }
        }

        [Fact]
        public void WhenInjectedInto_Complex_Decorator_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MyTestService>().As<IMyTestService>().ImportMembers();
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicDecorator)).As(typeof(IBasicService)).When.MeetsCondition(new CustomWhenInjectedInto(typeof(IMyTestService)));
            });

            var instance = container.Locate<IMyTestService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.ServiceAccessor);

            var basicInstance = instance.ServiceAccessor();

            Assert.NotNull(basicInstance);
            Assert.IsType<BasicDecorator>(basicInstance);

            var injectInstance = new MyTestService();

            container.Inject(injectInstance);

            Assert.NotNull(injectInstance);

            basicInstance = injectInstance.ServiceAccessor();

            Assert.NotNull(basicInstance);
            Assert.IsType<BasicDecorator>(basicInstance);
        }

        public class CustomWhenInjectedInto : ICompiledCondition
        {
            private readonly Func<Type, bool> _typeTest;

            public CustomWhenInjectedInto(params Type[] types)
            {
                if (types == null) throw new ArgumentNullException(nameof(types));

                _typeTest = type => TestTypes(type, types);
            }

            /// <summary>
            /// Test if strategy meets condition at configuration time
            /// </summary>
            /// <param name="strategy">strategy to test</param>
            /// <param name="staticInjectionContext">static injection context</param>
            /// <returns>meets condition</returns>
            public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
            {
                var targetInfo =
                    staticInjectionContext.InjectionStack.FirstOrDefault(
                        info => info.RequestingStrategy?.StrategyType == ActivationStrategyType.ExportStrategy);

                var type = targetInfo?.InjectionType ??
                           staticInjectionContext.InjectionStack.LastOrDefault()?.LocateType;

                return type != null && _typeTest(type);
            }

            /// <summary>
            /// Tests for if one type is based on another
            /// </summary>
            /// <param name="injectionType"></param>
            /// <param name="types"></param>
            /// <returns></returns>
            protected bool TestTypes(Type injectionType, Type[] types)
            {
                foreach (var type in types)
                {
                    if (ReflectionService.CheckTypeIsBasedOnAnotherType(injectionType, type))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion
    }
}
