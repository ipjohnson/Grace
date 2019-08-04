using Castle.DynamicProxy;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Intercept
{
    public class InterceptionTests
    {
        public interface ICalculator
        {
            int Add(int a, int b);
        }

        public class Calculator : ICalculator
        {
            public int Add(int a, int b)
            {
                return a + b;
            }
        }

        public class Interceptor : IInterceptor
        {
            public object[] Arguements { get; set; }

            public void Intercept(IInvocation invocation)
            {
                Arguements = invocation.Arguments;

                invocation.Proceed();
            }
        }

        [Fact]
        public void Intercept_Interface_Test()
        {
            var container = new DependencyInjectionContainer
            {
                c =>
                {
                    c.Export<Calculator>().As<ICalculator>();
                    c.Export<Interceptor>().Lifestyle.Singleton();
                    c.Intercept<ICalculator, Interceptor>();
                }
            };

            var calculator = container.Locate<ICalculator>();

            Assert.NotNull(calculator);

            var value = calculator.Add(2, 3);

            Assert.Equal(5, value);

            var interceptor = container.Locate<Interceptor>();

            Assert.NotNull(interceptor);
            Assert.NotNull(interceptor.Arguements);
            Assert.Equal(2, interceptor.Arguements.Length);
            Assert.Equal(2, interceptor.Arguements[0]);
            Assert.Equal(3, interceptor.Arguements[1]);
        }
        
        [Fact]
        public void Intercept_Singleton_Before_Lifestyle_Test()
        {
            var container = new DependencyInjectionContainer
            {
                c =>
                {
                    c.Export<Calculator>().As<ICalculator>().Lifestyle.SingletonPerScope();
                    c.Export<Interceptor>().Lifestyle.Singleton();
                    c.Intercept<ICalculator, Interceptor>();
                }
            };

            var calculator = container.Locate<ICalculator>();

            Assert.NotNull(calculator);

            var value = calculator.Add(2, 3);

            Assert.Equal(5, value);

            var interceptor = container.Locate<Interceptor>();

            Assert.NotNull(interceptor);
            Assert.NotNull(interceptor.Arguements);
            Assert.Equal(2, interceptor.Arguements.Length);
            Assert.Equal(2, interceptor.Arguements[0]);
            Assert.Equal(3, interceptor.Arguements[1]);
        }


        [Fact]
        public void Intercept_Singleton_After_Lifestyle_Test()
        {
            var container = new DependencyInjectionContainer
            {
                c =>
                {
                    c.Export<Calculator>().As<ICalculator>().Lifestyle.Singleton();
                    c.Export<Interceptor>().Lifestyle.Singleton();
                    c.Intercept<ICalculator, Interceptor>().ApplyAfterLifestyle();
                }
            };

            var calculator = container.Locate<ICalculator>();

            Assert.NotNull(calculator);

            var value = calculator.Add(2, 3);

            Assert.Equal(5, value);

            var interceptor = container.Locate<Interceptor>();

            Assert.NotNull(interceptor);
            Assert.NotNull(interceptor.Arguements);
            Assert.Equal(2, interceptor.Arguements.Length);
            Assert.Equal(2, interceptor.Arguements[0]);
            Assert.Equal(3, interceptor.Arguements[1]);
        }

        [Fact]
        public void InterceptUsingAttribute()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>();
                c.Export<SimpleObjectB>().As<ISimpleObject>();
            });

            container.InterceptAttribute<SimpleFilterAttribute, CustomInterceptor>();

            var all = container.LocateAll<ISimpleObject>();

            Assert.Equal(2, all.Count);
            Assert.IsNotType<SimpleObjectA>(all[0]);
            Assert.IsType<SimpleObjectB>(all[1]);

            Assert.Equal(CustomInterceptor.InterceptedString, all[0].TestString);
            Assert.Equal("B", all[1].TestString);
        }

        public class CustomInterceptor : IInterceptor
        {
            public CustomInterceptor()
            {

            }

            public const string InterceptedString = "intercepted";

            public void Intercept(IInvocation invocation)
            {
                invocation.ReturnValue = InterceptedString;
            }
        }
        
    }
}
