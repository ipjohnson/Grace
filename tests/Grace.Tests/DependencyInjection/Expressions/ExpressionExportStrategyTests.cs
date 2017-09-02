using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Expressions
{
    public class ExpressionExportStrategyTests
    {
        public class TwoArgClass
        {
            public TwoArgClass(IBasicService basicService, int intValue)
            {
                BasicService = basicService;
                IntValue = intValue;
            }

            public IBasicService BasicService { get; }

            public int IntValue { get; }
        }

        [Fact]
        public void ExpressionExport_LocateValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportExpression(() => new TwoArgClass(Arg.Locate<IBasicService>(), 5));
            });

            var instance = container.Locate<TwoArgClass>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.IntValue);
            Assert.IsType<BasicService>(instance.BasicService);
        }
    }
}
