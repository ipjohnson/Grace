using System;

namespace Grace.DependencyInjection.Lifestyle
{
    public class LifestylePicker<T> : ILifestylePicker<T>
    {
        private readonly T _returnValue;
        private readonly Action<ICompiledLifestyle> _addLifestyle;

        public LifestylePicker(T returnValue, Action<ICompiledLifestyle> addLifestyle)
        {
            _returnValue = returnValue;
            _addLifestyle = addLifestyle;
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
        /// Create one instance per named scope, 
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns></returns>
        public T SingletonPerNamedScope(string scopeName)
        {
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

        public T SingletonPerAncestor(Type ancestorType, bool locking = false)
        {
            _addLifestyle(new SingletonPerAncestor(ancestorType, locking));

            return _returnValue;
        }

        public T SingletonPerAncestor<TAncestor>(bool locking = false)
        {
            return SingletonPerAncestor(typeof(TAncestor), locking);
        }
    }
}
