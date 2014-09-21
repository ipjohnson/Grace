using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class IEnumerableExportStrategy<T> : BaseIEnumerableExportStrategy<T>
    {
        public override Type ActivationType
        {
            get { return typeof(IEnumerable<T>); }
        }

        public override Type InnerType
        {
            get { return typeof(T); }
        }

        protected override List<T> ActivateAll(IInjectionContext injectionContext,
            ExportStrategyFilter exportFilter,
            object locateKey,
            IExportStrategyCollection collection)
        {
            return collection.ActivateAll<T>(injectionContext, exportFilter, locateKey);
        }
    }
}
