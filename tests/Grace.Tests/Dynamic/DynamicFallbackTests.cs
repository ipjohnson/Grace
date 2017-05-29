using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Xunit;
using Grace.Dynamic;
using Grace.Tests.Classes.Simple;

namespace Grace.Tests.Dynamic
{
    public class DynamicFallbackTests
    {
        [Fact]
        public void DynamicCompile_Fallback()
        {
            var fallback = false;

            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s =>
                {
                    if (s.Contains("falling back"))
                    {
                        fallback = true;
                    }
                };
            }));

            container.Configure(c => c.ExportFactory<IInjectionContext,IDependentService<IBasicService>>(context =>
            {
                Assert.NotNull(context);

                return new DependentService<IBasicService>(new BasicService());
            }));

            var service = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(service);

            Assert.True(fallback);
        }
    }
}
