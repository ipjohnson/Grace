using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.MVC.DependencyInjection
{
    /// <summary>
    /// Provides per request lifestyles using http context
    /// </summary>
    public class WebSharedPerRequestLifestyleProvider : IPerRequestLifestyleProvider
    {
        /// <summary>
        /// Provide contianer
        /// </summary>
        /// <returns></returns>
        public ICompiledLifestyle ProvideLifestyle()
        {
            return new WebSharedPerRequestLifestyle();
        }
    }
}
