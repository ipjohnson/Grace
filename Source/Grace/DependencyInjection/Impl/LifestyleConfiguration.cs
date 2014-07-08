using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Lifestyle configuration object
    /// </summary>
    public class LifestyleConfiguration
    {
        private readonly IFluentExportStrategyConfiguration _strategyConfiguration;

        /// <summary>
        /// Default configuration object
        /// </summary>
        /// <param name="strategyConfiguration"></param>
        public LifestyleConfiguration(IFluentExportStrategyConfiguration strategyConfiguration)
        {
            _strategyConfiguration = strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration Singleton()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a weak singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration WeakSingleton()
        {
            _strategyConfiguration.UsingLifestyle(new WeakSingletonLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration SingletonPerInjection()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerInjectionContextLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration SingletonPerRequest()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerRequestLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration SingletonPerScope()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerScopeLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="metdata"></param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration SingletonPerAncestor<TAncestor>(object metdata = null)
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(typeof(TAncestor),metdata));

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="ancestorType">ancestor type</param>
        /// <param name="metdata">metadata object</param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration SingletonPerAncestor(Type ancestorType, object metdata = null)
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(ancestorType, metdata));

            return _strategyConfiguration;
        }
    }

    /// <summary>
    /// Lifestyle configuration object
    /// </summary>
    public class LifestyleConfiguration<T>
    {
        private readonly IFluentExportStrategyConfiguration<T> _strategyConfiguration;

        /// <summary>
        /// Default configuration object
        /// </summary>
        /// <param name="strategyConfiguration"></param>
        public LifestyleConfiguration(IFluentExportStrategyConfiguration<T> strategyConfiguration)
        {
            _strategyConfiguration = strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> Singleton()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a weak singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> WeakSingleton()
        {
            _strategyConfiguration.UsingLifestyle(new WeakSingletonLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> SingletonPerInjection()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerInjectionContextLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> SingletonPerRequest()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerRequestLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> SingletonPerScope()
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerScopeLifestyle());

            return _strategyConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="metdata"></param>
        /// <returns>configuration object</returns>
        public IFluentExportStrategyConfiguration<T> SingletonPerAncestor<TAncestor>(object metdata = null)
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
        public IFluentExportStrategyConfiguration<T> SingletonPerAncestor(Type ancestorType, object metdata = null)
        {
            _strategyConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(ancestorType, metdata));

            return _strategyConfiguration;
        }
    }
}
