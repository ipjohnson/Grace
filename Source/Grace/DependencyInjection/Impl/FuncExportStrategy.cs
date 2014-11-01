using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Export a Func(T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncExportStrategy<T> : ConfigurableExportStrategy
    {
        private readonly Func<IInjectionScope, IInjectionContext, T> _exportFunc;

        public FuncExportStrategy(Func<T> exportFunc) : this((s,c) => exportFunc())
        {

        }

        public FuncExportStrategy(Func<IInjectionScope,IInjectionContext,T> exportFunc) : base(typeof(T))
        {
            _exportFunc = exportFunc;
        }

        public override object Activate(IInjectionScope exportInjectionScope,
            IInjectionContext context,
            ExportStrategyFilter consider,
            object locateKey)
        {
            return _exportFunc(exportInjectionScope, context);
        }
    }
}
