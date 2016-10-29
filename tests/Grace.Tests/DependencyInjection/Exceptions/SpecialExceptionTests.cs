using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Xunit;

namespace Grace.Tests.DependencyInjection.Exceptions
{
    public class SpecialExceptionTests
    {
        public class InjectionDependentClass
        {
            private readonly IInjectionScope _injectionScope;

            public InjectionDependentClass(IInjectionScope injectionScope)
            {
                _injectionScope = injectionScope;
            }
        }

        [Fact]
        public void InjectionScopeInjectionException_Thrown_When_IInjectionScope_Requested()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ImportInjectionScopeException>(() => container.Locate<InjectionDependentClass>());
        }
    }
}
