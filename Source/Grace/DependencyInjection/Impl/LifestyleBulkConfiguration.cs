using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configures a set of exports
    /// </summary>
    public class LifestyleBulkConfiguration
    {
        private readonly IExportTypeSetConfiguration _typeSetConfiguration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="typeSetConfiguration">export configuration</param>
        public LifestyleBulkConfiguration(IExportTypeSetConfiguration typeSetConfiguration)
        {
            _typeSetConfiguration = typeSetConfiguration;
        }
        
        /// <summary>
        /// Applies a singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration Singleton()
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonLifestyle());

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Applies a weak singleton lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration WeakSingleton()
        {
            _typeSetConfiguration.UsingLifestyle(new WeakSingletonLifestyle());

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration SingletonPerInjection()
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonPerInjectionContextLifestyle());

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Applies a singleton per injection context lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration SingletonPerRequest()
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonPerRequestLifestyle());

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration SingletonPerScope()
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonPerScopeLifestyle());

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="metdata"></param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration SingletonPerAncestor<TAncestor>(object metdata = null)
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(typeof(TAncestor), metdata));

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Applies a singleton per scope lifestyle to the export
        /// </summary>
        /// <param name="ancestorType">ancestor type</param>
        /// <param name="metdata">metadata object</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration SingletonPerAncestor(Type ancestorType, object metdata = null)
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonPerAncestorLifestyle(ancestorType, metdata));

            return _typeSetConfiguration;
        }

        /// <summary>
        /// Exports a type that will be shared per named scope
        /// </summary>
        /// <param name="namedScope"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration SingletonPerNamedScope(string namedScope)
        {
            _typeSetConfiguration.UsingLifestyle(new SingletonPerNamedScopeLifestyle(namedScope));

            return _typeSetConfiguration;
        }
    }
}
