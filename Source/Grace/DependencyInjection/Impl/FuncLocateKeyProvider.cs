using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Locate key Provider that takes a func
    /// </summary>
    public class FuncLocateKeyProvider : ILocateKeyValueProvider
    {
        private readonly Func<IInjectionScope,IInjectionContext, Type, object> locateKeyFunc; 
 
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="locateKeyFunc">locate func</param>
        public FuncLocateKeyProvider(Func<Type, object> locateKeyFunc)
        {
            this.locateKeyFunc = (scope,context,t) => locateKeyFunc(t);
        }

        /// <summary>
        /// Constructor that takes a full locate key func
        /// </summary>
        /// <param name="locateKeyFunc"></param>
        public FuncLocateKeyProvider(Func<IInjectionScope, IInjectionContext, Type, object> locateKeyFunc)
        {
            this.locateKeyFunc =  locateKeyFunc;
        }

        /// <summary>
        /// Provide locate key
        /// </summary>
        /// <param name="injectionScope">current injection scope</param>
        /// <param name="context">current locate context</param>
        /// <param name="activationType">type being activated</param>
        /// <returns>locate key</returns>
        public object ProvideValue(IInjectionScope injectionScope, IInjectionContext context, Type activationType)
        {
            return locateKeyFunc(injectionScope,context,activationType);
        }
    }
}
