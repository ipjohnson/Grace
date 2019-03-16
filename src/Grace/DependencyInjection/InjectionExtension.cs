using System;
using Grace.Data;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// C# extenion class to inject object
    /// </summary>
    public static class InjectionExtension
    {
        /// <summary>
        /// Inject instance with dependencies
        /// </summary>
        /// <param name="scope">export locator scope</param>
        /// <param name="instance">instance to inject</param>
        /// <param name="extraData">extra data, can be null</param>
        public static void Inject(this IExportLocatorScope scope, object instance, object extraData = null)
        {
            if (scope == null) throw new ArgumentNullException(nameof(scope));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var parentScope = scope;

            while (parentScope.Parent != null)
            {
                parentScope = parentScope.Parent;
            }

            var injectionScope = (IInjectionScope)parentScope;

            var value = parentScope.GetExtraDataOrDefaultValue<InjectionHashTreeHolder>(typeof(InjectionHashTreeHolder)) ??
                        (InjectionHashTreeHolder)parentScope.SetExtraData(typeof(InjectionHashTreeHolder), new InjectionHashTreeHolder(), false);

            var instanceType = instance.GetType();

            var injectionDelegate = value.Delegates.GetValueOrDefault(instanceType);

            if (injectionDelegate == null)
            {
                injectionDelegate =
                    injectionScope.StrategyCompiler.CreateInjectionDelegate(injectionScope, instanceType);

                injectionDelegate =
                    ImmutableHashTree.ThreadSafeAdd(ref value.Delegates, instanceType, injectionDelegate);
            }

            IInjectionContext context = null;

            if (extraData != null)
            {
                context = injectionScope.CreateContext(extraData);
            }

            injectionDelegate(scope, scope, context, instance);
        }

        /// <summary>
        /// 
        /// </summary>
        private class InjectionHashTreeHolder
        {
            public ImmutableHashTree<Type, InjectionStrategyDelegate> Delegates = ImmutableHashTree<Type, InjectionStrategyDelegate>.Empty;
        }
    }
}
