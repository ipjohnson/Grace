using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Classes implementing this interface can be used to provide a key at locate time
    /// </summary>
    public interface ILocateKeyValueProvider
    {
        /// <summary>
        /// Provide locate key
        /// </summary>
        /// <param name="injectionScope">current injection scope</param>
        /// <param name="context">current locate context</param>
        /// <param name="activationType">type being activated</param>
        /// <returns>locate key</returns>
        object ProvideValue(IInjectionScope injectionScope, IInjectionContext context, Type activationType);
    }
}
