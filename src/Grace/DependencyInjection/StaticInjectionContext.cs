using System;
using System.Collections.Generic;
using System.Linq;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection
{
    public class StaticInjectionContext
    {
        public StaticInjectionContext(Type activationType, IEnumerable<InjectionTargetInfo> injectionStack = null)
        {
            ActivationType = activationType;

            if (injectionStack == null)
            {
                InjectionStack = ImmutableLinkedList<InjectionTargetInfo>.Empty;
            }
            else
            {
                var array = injectionStack.ToArray();

                TargetInfo = array.FirstOrDefault();

                InjectionStack = array;
            }
        }

        public Type ActivationType { get; }

        public InjectionTargetInfo TargetInfo { get; }

        public IEnumerable<InjectionTargetInfo> InjectionStack { get; }
    }
}
