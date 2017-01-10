using System;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// This is the DI container for Grace implementation
    /// </summary>
    public class ImplementationFactory
    {
        private ImmutableHashTree<Type, object> _factories = ImmutableHashTree<Type, object>.Empty;

        /// <summary>
        /// Injection scope
        /// </summary>
        public IInjectionScope InjectionScope { get; set; }

        /// <summary>
        /// Export an instance of a type
        /// </summary>
        /// <typeparam name="T">type being exported</typeparam>
        /// <param name="exportFunc">export func</param>
        public void ExportInstance<T>(Func<ImplementationFactory, T> exportFunc)
        {
            _factories = _factories.Add(typeof(T), exportFunc, (o, n) => n);
        }

        /// <summary>
        /// Export a singleton
        /// </summary>
        /// <typeparam name="T">type being exported</typeparam>
        /// <param name="exportFunc">export func</param>
        public void ExportSingleton<T>(Func<ImplementationFactory, T> exportFunc)
        {
            var tValue = default(T);

            Func<ImplementationFactory, T> singletonFunc =
                f =>
                {
                    if (Equals(tValue, default(T)))
                    {
                        tValue = exportFunc(f);
                    }

                    return tValue;
                };

            _factories = _factories.Add(typeof(T), singletonFunc, (o, n) => n);
        }

        /// <summary>
        /// Locate instances
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Locate<T>()
        {
            var func = _factories.GetValueOrDefault(typeof(T)) as Func<ImplementationFactory, T>;

            if (func == null)
            {
                throw new Exception($"Could not locate type of {typeof(T).FullName}");
            }

            return func(this);
        }

        /// <summary>
        /// Clone implementation factory
        /// </summary>
        /// <returns></returns>
        public ImplementationFactory Clone()
        {
            return new ImplementationFactory { _factories = _factories };
        }
    }
}
