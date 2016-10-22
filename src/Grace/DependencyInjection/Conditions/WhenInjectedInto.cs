using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data;
using Grace.Utilities;

namespace Grace.DependencyInjection.Conditions
{
    public class WhenInjectedInto : ICompiledCondition
    {
        private readonly Type[] _types;

        public WhenInjectedInto(params Type[] types)
        {
            _types = types;
        }

        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            var targetInfo = staticInjectionContext.TargetInfo;

            if (targetInfo != null)
            {
                foreach (var type in _types)
                {
                    if (ReflectionService.CheckTypeIsBasedOnAnotherType(targetInfo.InjectionType, type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
