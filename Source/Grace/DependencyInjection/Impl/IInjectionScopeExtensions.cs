using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Grace.DependencyInjection.Impl
{
    public static class IInjectionScopeExtensions
    {
        /// <summary>
        /// Returns the root scope of the injection scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [NotNull]
        public static IInjectionScope RootScope(this IInjectionScope scope)
        {
            while (scope.ParentScope != null)
            {
                scope = scope.ParentScope;
            }

            return scope;
        }
    }
}
