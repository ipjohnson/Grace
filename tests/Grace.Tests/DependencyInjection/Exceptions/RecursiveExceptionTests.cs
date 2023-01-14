using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Xunit;

namespace Grace.Tests.DependencyInjection.Exceptions
{
    public class RecursiveExceptionTests
    {
        public class RecursiveClass1
        {
            private readonly RecursiveClass2 _class2;

            public RecursiveClass1(RecursiveClass2 class2)
            {
                _class2 = class2;
            }
        }

        public class RecursiveClass2
        {
            private readonly RecursiveClass1 _class1;

            public RecursiveClass2(RecursiveClass1 class1)
            {
                _class1 = class1;
            }
        }

        [Fact]
        public void RecursiveException_Throws_Exception()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<RecursiveLocateException>(() => container.Locate<RecursiveClass1>());
        }
    }
}
