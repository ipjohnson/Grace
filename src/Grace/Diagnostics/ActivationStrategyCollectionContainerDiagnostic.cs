using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Diagnostic class used for visual studio debugging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public class ActivationStrategyCollectionContainerDiagnostic<T> where T : IActivationStrategy
    {
        private readonly IActivationStrategyCollectionContainer<T> _container;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="container"></param>
        public ActivationStrategyCollectionContainerDiagnostic(IActivationStrategyCollectionContainer<T> container)
        {
            _container = container;
        }

        /// <summary>
        /// The number of strategies
        /// </summary>
        public int Count => _container.GetAllStrategies().Count();

        /// <summary>
        /// The number of types exported under
        /// </summary>
        public int Types => _container.GetActivationTypes().Count();

        
        private string DebugDisplayString => "Count: " + Count;
    }
}
