using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    public class OwnedListExportStrategy<T> : BaseIEnumerableExportStrategy<Owned<T>> where T : class 
    {
        public override Type ActivationType
        {
            get { return typeof(IEnumerable<Owned<T>>); }
        }

        public override Type InnerType
        {
            get { return typeof(T); }
        }

        protected override List<Owned<T>> ActivateAll(IInjectionContext injectionContext,
            ExportStrategyFilter exportFilter,
            object locateKey,
            IExportStrategyCollection collection)
        {
            return collection.ActivateAllOwned<Owned<T>, T>(injectionContext, exportFilter, locateKey);
        }
    }   
}