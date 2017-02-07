using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Similar to Owned with the difference being a new scope is created and used to resolve instance 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Scoped<T> : IDisposable
    {
        private IExportLocatorScope _scope;
        private string _scopeName;
        private IExportLocatorScope _childScope;
        private T _instance;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="scopeName"></param>
        public Scoped(IExportLocatorScope scope, string scopeName = null)
        {
            _scope = scope;
            _scopeName = scopeName;
        }

        /// <summary>
        /// Instance 
        /// </summary>
        public T Instance
        {
            get
            {
                if (_childScope == null)
                {
                    _childScope = _scope.BeginLifetimeScope(_scopeName);

                    _instance = _childScope.Locate<T>();
                }

                return _instance;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _childScope?.Dispose();
        }
    }
}
