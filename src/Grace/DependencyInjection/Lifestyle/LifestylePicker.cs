using System;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Class used to configure lifestyles 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LifestylePicker<T> : ILifestylePicker<T>
    {
        private readonly T _returnValue;
        private readonly Action<ICompiledLifestyle> _addLifestyle;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="returnValue"></param>
        /// <param name="addLifestyle"></param>
        public LifestylePicker(T returnValue, Action<ICompiledLifestyle> addLifestyle)
        {
            _returnValue = returnValue;
            _addLifestyle = addLifestyle;
        }

        /// <summary>
        /// Use custom lifestyle
        /// </summary>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public T Custom(ICompiledLifestyle lifestyle)
        {
            if (lifestyle != null)
            {
                _addLifestyle(lifestyle);
            }

            return _returnValue;
        }

        /// <summary>
        /// Create one instance for the container including all child scopes and lifetime scopes
        /// </summary>
        /// <returns>configuration</returns>
        public T Singleton()
        {
            _addLifestyle(new SingletonLifestyle());

            return _returnValue;
        }


        /// <summary>
        /// Create one instance per object graph
        /// </summary>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the object graph, set to true only if you believe there are multiple threads accessing one object graph</param>
        /// <returns></returns>
        public T SingletonPerObjectGraph(bool locking = false)
        {
            _addLifestyle(new SingletonPerObjectGraph(locking));

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per request, usually a lock is not needed
        /// </summary>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the request, set to true only if you believe there are multple threads accesing this export</param>
        /// <returns></returns>
        public T SingletonPerRequest(bool locking = false)
        {
            _addLifestyle(new SingletonPerRequestLifestyle());

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per scope, by default no lock is used if you think it's possible multiple threads will resolve form a scope then set locking to true
        /// </summary>
        /// <param name="locking">false by default, set to true if you have multiple threads resolving from the </param>
        /// <returns></returns>
        public T SingletonPerScope(bool locking = false)
        {
            _addLifestyle(new SingletonPerScopeLifestyle(locking));

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per key
        /// </summary>
        /// <param name="keyFunc"></param>
        /// <returns></returns>
        public T SingletonPerKey(Func<IExportLocatorScope, IInjectionContext, object> keyFunc)
        {
            _addLifestyle(new SingletonPerKeyLifestyle(keyFunc));

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per named scope, 
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns></returns>
        public T SingletonPerNamedScope(string scopeName)
        {
            if (scopeName == null) throw new ArgumentNullException(nameof(scopeName));

            _addLifestyle(new SingletonPerNamedScopeLifestyle(scopeName));

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per container but don't hold a strong reference
        /// </summary>
        /// <returns></returns>
        public T WeakSingleton()
        {
            _addLifestyle(new WeakSingletonLifestyle());

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per ancestor in the object graph
        /// </summary>
        /// <param name="ancestorType">ancestor type to scope to</param>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the object graph, set to true only if you believe there are multiple threads accessing one object graph</param>
        /// <returns></returns>
        public T SingletonPerAncestor(Type ancestorType, bool locking = false)
        {
            if (ancestorType == null) throw new ArgumentNullException(nameof(ancestorType));

            _addLifestyle(new SingletonPerAncestor(ancestorType, locking));

            return _returnValue;
        }

        /// <summary>
        /// Create one instance per ancestor in the object graph
        /// </summary>
        /// <typeparam name="TAncestor">ancestor type to scope to</typeparam>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the object graph, set to true only if you believe there are multiple threads accessing one object graph</param>
        /// <returns></returns>
        public T SingletonPerAncestor<TAncestor>(bool locking = false)
        {
            return SingletonPerAncestor(typeof(TAncestor), locking);
        }
    }
}
