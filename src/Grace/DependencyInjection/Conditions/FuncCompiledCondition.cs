using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Conditions
{
    public class FuncCompiledCondition : ICompiledCondition
    {
        private readonly Func<IActivationStrategy, StaticInjectionContext, bool> _condition;

        public FuncCompiledCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition)
        {
            _condition = condition;
        }

        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            return _condition(strategy, staticInjectionContext);
        }
    }
}
