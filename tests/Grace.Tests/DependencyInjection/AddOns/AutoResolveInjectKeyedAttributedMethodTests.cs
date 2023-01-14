using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class AutoResolveInjectKeyedAttributedMethodTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AutoResolveInjectKeyedAttributedMethodTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ComplexRegistrationTest()
        {
            var container = new DependencyInjectionContainer(c =>
            {
                c.AutoRegisterUnknown = true;
                c.Trace = (s => { _testOutputHelper.WriteLine(s); });
            });

            container.Configure((c) => 
            {
                c.Export<Service>().AsKeyed<IService>("ServiceM");

                c.Export<ServiceAlt>().AsKeyed<IService>("ServiceA");
            });


            var instance = container.Locate<ServiceMain>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Service);
            Assert.Contains("Alt", instance.Service.GetString());
        }

        interface IService
        {
            string GetString();
        }

        class Service : IService
        {
            public virtual string GetString()
            {
                return "Service";
            }
        }

        class ServiceAlt : IService
        {
            public string GetString()
            {
                return "Service AlternativeService";
            }
        }

        class ServiceMain
        {
            public IService Service { get; set; }
            
            [Import]
            public void Recieve([Import(Key = "ServiceA")]IService service)
            {
                Console.WriteLine("Import through method");
                Service = service;
            }
        }

    }
}
