using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Trace
{
    public class TraceTests
    {
        [Fact]
        public void CustomTrace()
        {
            StringBuilder builder = new StringBuilder();
            var container = new DependencyInjectionContainer(c => c.Trace = message => builder.AppendLine(message));
            
            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.Export<BasicService>().As<IBasicService>();
            });

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);

            var diagnosticOutput = builder.ToString();

            Assert.Contains("BasicService", diagnosticOutput);
            Assert.Contains("value", diagnosticOutput);
        }
         
    }
}
