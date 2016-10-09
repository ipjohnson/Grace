using Grace.Data.Immutable;
using System;

namespace Grace.DependencyInjection.Impl
{
    public class ImplementationFactory
    {
        private ImmutableHashTree<Type, object> _factories = ImmutableHashTree<Type, object>.Empty;

        public IInjectionScope InjectionScope { get; set; }

        public void ExportInstance<T>(Func<ImplementationFactory, T> exportFunc)
        {
            _factories = _factories.Add(typeof(T), exportFunc, (o, n) => n);
        }

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

        public T Locate<T>()
        {
            var func = _factories.GetValueOrDefault(typeof(T)) as Func<ImplementationFactory, T>;

            if (func == null)
            {
                throw new Exception($"Could not locate type of {typeof(T).FullName}");
            }

            return func(this);
        }

        public ImplementationFactory Clone()
        {
            return new ImplementationFactory { _factories = _factories };
        }
    }
}
