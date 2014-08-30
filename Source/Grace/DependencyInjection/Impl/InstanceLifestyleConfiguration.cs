using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configures an exported instance lifestyle
    /// </summary>
    public class InstanceLifestyleConfiguration<T>
    {
        private readonly IFluentExportInstanceConfiguration<T> _strategyConfiguration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategyConfiguration"></param>
        public InstanceLifestyleConfiguration(IFluentExportInstanceConfiguration<T> strategyConfiguration)
        {
            _strategyConfiguration = strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> Singleton()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a weak singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> WeakSingleton()
        {
            _strategyConfiguration.UsingLifestyle(new WeakSingletonLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> SingletonPerInjection()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerInjectionContextLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> SingletonPerRequest()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerRequestLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> SingletonPerScope()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerScopeLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="metdata"></param>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> SingletonPerAncestor<TAncestor>(object metdata = null)
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(typeof(TAncestor), metdata));

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="ancestorType">ancestor type</param>
        /// <param name="metdata">metadata object</param>
        /// <returns>configuration object</returns>
        public IFluentExportInstanceConfiguration<T> SingletonPerAncestor(Type ancestorType, object metdata = null)
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(ancestorType, metdata));

            return _strategyConfiguration;
        }

        /// <summary>
        /// Exports will create a singleton per named scope
        /// </summary>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        public IFluentExportInstanceConfiguration<T> SingletonPerNamedScope(string scopeName)
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerNamedScopeLifestyle(scopeName));

            return _strategyConfiguration;
        }
    }
}
