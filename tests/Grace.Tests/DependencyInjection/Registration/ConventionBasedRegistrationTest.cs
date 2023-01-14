using System.Reflection;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class ConventionBasedRegistrationTest
    {
        [Fact]
        public void ConventionBasedNameTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.AddInjectionValueProvider(new ConventionBasedValueProvider());
                c.Export<MultipleService1>().AsKeyed<IMultipleService>("serviceA");
                c.Export<MultipleService2>().AsKeyed<IMultipleService>("serviceB");
                c.Export(typeof(ConventionBasedRegistrationTest).Assembly.ExportedTypes).ByInterfaces();
            });

            var instance = container.Locate<IConventionBasedService>();

            Assert.NotNull(instance);
            Assert.IsType<MultipleService1>(instance.ServiceA);
            Assert.IsType<MultipleService2>(instance.ServiceB);
        }

        /// <summary>
        /// This class is used to add a key lookup for IMultipleService dependency
        /// </summary>
        public class ConventionBasedValueProvider : IInjectionValueProvider
        {
            public bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
            {
                return false;
            }

            public IActivationExpressionResult GetExpressionResult(IInjectionScope scope, IActivationExpressionRequest request)
            {
                // test if it's the interface we're looking for and it's for a parameter
                if (request.ActivationType == typeof(IMultipleService) && 
                    request.Info is ParameterInfo memberInfo)
                {
                    // set the locate key automatically to be the parameter name
                    request.SetLocateKey(memberInfo.Name);
                }

                return null;
            }
        }
    }
}
