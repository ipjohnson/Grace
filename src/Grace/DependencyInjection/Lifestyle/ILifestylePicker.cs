namespace Grace.DependencyInjection.Lifestyle
{
    public interface ILifestylePicker<T>
    {
        /// <summary>
        /// Create one instance for the container including all child scopes and lifetime scopes
        /// </summary>
        /// <returns>configuration</returns>
        T Singleton();

        /// <summary>
        /// Create one instance per scope, by default no lock is used if you think it's possible multiple threads will resolve form a scope then set locking to true
        /// </summary>
        /// <param name="locking">false by default, set to true if you have multiple threads resolving from the smae scope </param>
        /// <returns></returns>
        T SingletonPerScope(bool locking = false);

        /// <summary>
        /// Create one instance per named scope, 
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns></returns>
        T SingletonPerNamedScope(string scopeName);

        /// <summary>
        /// Create one instance per container but don't hold a strong reference
        /// </summary>
        /// <returns></returns>
        T WeakSingleton();
    }
}