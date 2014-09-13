using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Generic export strategy inspector
    /// </summary>
    public class FuncExportStrategyInspector : IExportStrategyInspector
    {
        private readonly Action<IExportStrategy> _inspectAction;

        /// <summary>
        /// Default export strategy inspector
        /// </summary>
        /// <param name="inspectAction">inspect action</param>
        public FuncExportStrategyInspector(Action<IExportStrategy> inspectAction)
        {
            _inspectAction = inspectAction;
        }
        
        /// <summary>
        /// Inspect export strategy
        /// </summary>
        /// <param name="exportStrategy">export strategy</param>
        public void Inspect(IExportStrategy exportStrategy)
        {
            _inspectAction(exportStrategy);
        }
    }
}
