using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a class that can locate types, classes should not implement this interface directly rather IExportLocatorScope
    /// </summary>
    public interface ILocatorService : IServiceProvider
    {
        /// <summary>
        /// Can Locator type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="consider"></param>
        /// <param name="key">key to use while locating</param>
        bool CanLocate(Type type, ActivationStrategyFilter consider = null, object key = null);

        /// <summary>
        /// Locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>located instance</returns>
        object Locate(Type type);

        /// <summary>
        /// Locate type or return default value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        object LocateOrDefault(Type type, object defaultValue);

        /// <summary>
        /// Locate type
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <returns>located instance</returns>
        T Locate<T>();

        /// <summary>
        /// Locate or return default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        T LocateOrDefault<T>(T defaultValue = default(T));

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data to be used during construction</param>
        /// <param name="consider">strategy filter</param>
        /// <param name="withKey">key to use for locating type</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        object Locate(Type type, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <param name="extraData">extra data</param>
        /// <param name="consider"></param>
        /// <param name="withKey">key to use during construction</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        T Locate<T>(object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        /// <summary>
        /// Locate all instances of a specific type
        /// </summary>
        /// <param name="type">type ot locate</param>
        /// <param name="extraData">extra data to be used while locating</param>
        /// <param name="consider">strategy filter</param>
        /// <param name="comparer">comparer to use to sort collection</param>
        /// <param name="withKey">key to use during construction</param>
        List<object> LocateAll(Type type, object extraData = null, ActivationStrategyFilter consider = null, IComparer<object> comparer = null, object withKey = null);

        /// <summary>
        /// Locate all of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">type to locate, can be null</param>
        /// <param name="extraData">extra data to use during locate</param>
        /// <param name="consider"></param>
        /// <param name="comparer"></param>
        /// <param name="withKey">key to use during construction</param>
        List<T> LocateAll<T>(Type type = null, object extraData = null, ActivationStrategyFilter consider = null, IComparer<T> comparer = null, object withKey = null);

        /// <summary>
        /// Try to locate an export by type
        /// </summary>
        /// <typeparam name="T">locate type</typeparam>
        /// <param name="value">out value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <param name="isDynamic"></param>
        bool TryLocate<T>(out T value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        /// <summary>
        /// try to locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="value">located value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <param name="isDynamic"></param>
        bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        /// <summary>
        /// Locate by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        object LocateByName(string name, object extraData = null, ActivationStrategyFilter consider = null);

        /// <summary>
        /// Locate all by specific name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        List<object> LocateAllByName(string name, object extraData = null, ActivationStrategyFilter consider = null);

        /// <summary>
        /// Try to locate by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        bool TryLocateByName(string name, out object value, object extraData = null, ActivationStrategyFilter consider = null);
    }
}
