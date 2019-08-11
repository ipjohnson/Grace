using System;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Configuration object for picking a lifestyle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILifestylePicker<out T>
    {
        /// <summary>
        /// Use custom lifestyle
        /// </summary>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        T Custom(ICompiledLifestyle lifestyle);

        /// <summary>
        /// Create one instance for the container including all child scopes and lifetime scopes
        /// </summary>
        /// <returns>configuration</returns>
        T Singleton();

        /// <summary>
        /// Create one instance per ancestor in the object graph
        /// </summary>
        /// <param name="ancestorType">ancestor type to scope to</param>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the object graph, set to true only if you believe there are multiple threads accessing one object graph</param>
        /// <returns></returns>
        T SingletonPerAncestor(Type ancestorType, bool locking = false);

        /// <summary>
        /// Create one instance per ancestor in the object graph
        /// </summary>
        /// <typeparam name="TAncestor">ancestor type to scope to</typeparam>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the object graph, set to true only if you believe there are multiple threads accessing one object graph</param>
        /// <returns></returns>
        T SingletonPerAncestor<TAncestor>(bool locking = false);

        /// <summary>
        /// Create one instance per named scope, an exception is thrown if a scope by that name can't be found
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns></returns>
        T SingletonPerNamedScope(string scopeName);

        /// <summary>
        /// Create one instance per object graph
        /// </summary>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the object graph, set to true only if you believe there are multiple threads accessing one object graph</param>
        /// <returns></returns>
        T SingletonPerObjectGraph(bool locking = false);

        /// <summary>
        /// Create one instance per request, usually a lock is not needed
        /// </summary>
        /// <param name="locking">by default no lock is used and it's assumed there is only one thread of execution in the request, set to true only if you believe there are multple threads accesing this export</param>
        /// <returns></returns>
        T SingletonPerRequest(bool locking = false);

        /// <summary>
        /// Create one instance per scope, by default no lock is used if you think it's possible multiple threads will resolve this from the same scope then set to true
        /// </summary>
        /// <param name="locking">false by default, set to true if you have multiple threads resolving from the same scope </param>
        /// <returns></returns>
        T SingletonPerScope(bool locking = false);

        /// <summary>
        /// Singleton per key
        /// </summary>
        /// <param name="keyFunc"></param>
        /// <returns></returns>
        T SingletonPerKey(Func<IExportLocatorScope, IInjectionContext, object> keyFunc);

        /// <summary>
        /// Create one instance per container but don't hold a strong reference
        /// </summary>
        /// <returns></returns>
        T WeakSingleton();
    }
}