using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Exceptions
{
    public class LocateExceptionTests
    {
        [Fact]
        public void LocateException_Thrown_When_Dependency_Missing()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<LocateException>(() => container.Locate<DependentService<IBasicService>>());
        }
    }
}
