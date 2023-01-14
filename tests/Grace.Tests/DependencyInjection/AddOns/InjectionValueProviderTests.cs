using System.Linq.Expressions;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class InjectionValueProviderTests
    {
        public class ValueProvider : IInjectionValueProvider
        {
            public IBasicService BasicService { get; set; }

            /// <summary>
            ///  Can a value be located for this request
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <returns></returns>
            public bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
            {
                return request.ActivationType == typeof(IBasicService);
            }

            public IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request)
            {
                if (request.ActivationType == typeof(IBasicService))
                {
                    return request.Services.Compiler.CreateNewResult(request, Expression.Constant(BasicService));
                }

                return null;
            }
        }

        [Fact]
        public void InjectionValueProvider_Test()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService { Count = 15 };

            container.Configure(c =>
            {
                c.AddInjectionValueProvider(new ValueProvider { BasicService = basicService });
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.Export<MultipleService1>().As<IMultipleService>();
            });

            var valueProviderInstance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(valueProviderInstance);
            Assert.NotNull(valueProviderInstance.Value);
            Assert.Equal(15, valueProviderInstance.Value.Count);

            var nonValueProviderInstance = container.Locate<IDependentService<IMultipleService>>();

            Assert.NotNull(valueProviderInstance);
            Assert.NotNull(nonValueProviderInstance.Value);
        }
    }
}
