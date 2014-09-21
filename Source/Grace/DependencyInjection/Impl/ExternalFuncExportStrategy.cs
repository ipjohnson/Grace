using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class ExternalFuncExportStrategy<T> : ConfigurableExportStrategy
    {
        private readonly Func<T> _func;

        public ExternalFuncExportStrategy(Func<T> func, Type exportType = null)
            : base(exportType ?? typeof(T))
        {
            _func = func;
        }

        public override object Activate(IInjectionScope exportInjectionScope,
            IInjectionContext context,
            ExportStrategyFilter consider,
            object locateKey)
        {
            if (Lifestyle != null)
            {
                return Lifestyle.Locate(CreateDelegate, exportInjectionScope, context, this);
            }

            return CreateDelegate(exportInjectionScope, context);
        }

        private object CreateDelegate(IInjectionScope injectionscope, IInjectionContext context)
        {
            return _func();
        }
    }
}
