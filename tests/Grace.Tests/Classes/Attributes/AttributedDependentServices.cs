using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Tests.Classes.Simple;

namespace Grace.Tests.Classes.Attributes
{
    [SomeTest]
    public class AttributedDependentService<T> : IDependentService<T>
    {
        public AttributedDependentService(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }

    [SomeTest(TestValue = 10)]
    public class OtherAttributedDependentService<T> : IDependentService<T>
    {
        public OtherAttributedDependentService(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
