using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Prioritize types that match
    /// </summary>
    public class PrioritizeTypesThatInspector : IExportStrategyInspector
    {
        private readonly int _priority;
        private readonly Func<Type, bool> _typesThatFunc;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="typesThatFunc"></param>
        /// <param name="priority"></param>
        public PrioritizeTypesThatInspector(Func<Type, bool> typesThatFunc, int priority)
        {
            _typesThatFunc = typesThatFunc;
            _priority = priority;
        }

        /// <summary>
        /// Inspect strategies
        /// </summary>
        /// <param name="exportStrategy">strategy</param>
        public void Inspect(IExportStrategy exportStrategy)
        {
            IConfigurableExportStrategy configurableExport = exportStrategy as IConfigurableExportStrategy;

            if (configurableExport != null &&
                _typesThatFunc(exportStrategy.ActivationType))
            {
                configurableExport.SetPriority(_priority);
            }
        }
    }
}
