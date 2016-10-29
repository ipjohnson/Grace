using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    namespace AncestorClasses
    {
        public class Root
        {
            public Root(SomeClass instance1, SomeClass instance2)
            {
                Instance1 = instance1;
                Instance2 = instance2;
            }

            public SomeClass Instance1 { get; }

            public SomeClass Instance2 { get; }
        }

        public class SomeClass
        {
            public SomeClass(LeafClass leaf)
            {
                Leaf = leaf;
            }

            public LeafClass Leaf { get; }
        }

        public class LeafClass
        {
            public LeafClass(SharedClass shared1, SharedClass shared2)
            {
                Shared1 = shared1;
                Shared2 = shared2;
            }

            public SharedClass Shared1 { get; }

            public SharedClass Shared2 { get; }
        }

        public class SharedClass
        {

        }
    }
}
