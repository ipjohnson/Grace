using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExtraData
{
    public class BlackListExtraDataTests
    {
        public class Top
        {
            public Top(Sub subInstance)
            {
                SubInstance = subInstance;
            }

            public Sub SubInstance { get;   }
        }

        public class Sub
        {

        }


        [Fact]
        public void BlackListModelExtraData()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(block => block.ExcludeTypeFromAutoRegistration(typeof(Sub)));

            var subInstance = new Sub();

            var topInstance = container.Locate<Top>(new {subInstance});

            Assert.Equal(subInstance, topInstance.SubInstance);
        }
    }
}
