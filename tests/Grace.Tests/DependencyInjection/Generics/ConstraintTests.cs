using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Generics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{
    public class ConstraintTests
    {
        [Fact]
        public void Generic_Resolve_All_With_Constraint_Failing()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export(typeof(GenericConstraintServiceA<>)).As(typeof(IGenericConstraintService<>));
                c.Export(typeof(GenericConstraintBasicService<>)).As(typeof(IGenericConstraintService<>));
            });

            var instances = container.LocateAll<IGenericConstraintService<IMultipleService>>();

            Assert.NotNull(instances);
            Assert.Equal(1, instances.Count);
        }
    }
}
