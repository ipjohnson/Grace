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
        /// Create one instance per named scope, 
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
        /// Create one instance per scope, by default no lock is used if you think it's possible multiple threads will resolve form a scope then set locking to true
        /// </summary>
        /// <param name="locking">false by default, set to true if you have multiple threads resolving from the smae scope </param>
        /// <returns></returns>
        T SingletonPerScope(bool locking = false);

        /// <summary>
        /// Create one instance per container but don't hold a strong reference
        /// </summary>
        /// <returns></returns>
        T WeakSingleton();
    }
}