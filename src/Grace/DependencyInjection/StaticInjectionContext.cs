using System;
using System.Collections.Generic;
using System.Linq;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// hold static information about the injection
    /// </summary>
    public class StaticInjectionContext
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionStack"></param>
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

        /// <summary>
        /// Type being activated
        /// </summary>
        public Type ActivationType { get; }

        /// <summary>
        /// Target information
        /// </summary>
        public InjectionTargetInfo TargetInfo { get; }

        /// <summary>
        /// Injection context stack
        /// </summary>
        public IEnumerable<InjectionTargetInfo> InjectionStack { get; }
    }
}
