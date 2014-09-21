using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public class LazyListExportStrategy<T> : BaseIEnumerableExportStrategy<Lazy<T>>
    {
        public override Type ActivationType
        {
            get { return typeof(IEnumerable<Lazy<T>>); }
        }

        public override Type InnerType
        {
            get { return typeof(T); }
        }

        protected override List<Lazy<T>> ActivateAll(IInjectionContext injectionContext,
            ExportStrategyFilter exportFilter,
            object locateKey,
            IExportStrategyCollection collection)
        {
            return collection.ActivateAllLazy<Lazy<T>, T>(injectionContext, exportFilter, locateKey);
        }
    }
}
