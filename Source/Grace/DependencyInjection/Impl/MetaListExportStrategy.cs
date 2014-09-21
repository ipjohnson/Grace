using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class MetaListExportStrategy<T> : BaseIEnumerableExportStrategy<Meta<T>>
    {
        public override Type ActivationType
        {
            get { return typeof(IEnumerable<Meta<T>>); }
        }

        public override Type InnerType
        {
            get { return typeof(T); }
        }

        protected override List<Meta<T>> ActivateAll(IInjectionContext injectionContext,
            ExportStrategyFilter exportFilter,
            object locateKey,
            IExportStrategyCollection collection)
        {
            return collection.ActivateAllMeta<Meta<T>, T>(injectionContext, exportFilter, locateKey);
        }
    }
}
