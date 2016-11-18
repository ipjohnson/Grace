using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Describes a missing dependency for an activation strategy
    /// </summary>
    public class MissingActivationStrategyDependency
    {
        public ActivationStrategyDependency Dependency { get; }
    }
}
