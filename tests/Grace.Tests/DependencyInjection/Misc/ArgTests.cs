using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Misc
{
    public class ArgTests
    {
        [Fact]
        public void Arg_Any_Test()
        {
            Assert.Equal(0, Arg.Any<int>());
        }
    }
}
