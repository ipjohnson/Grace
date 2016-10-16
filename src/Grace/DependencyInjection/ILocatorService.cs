using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a class that can locate types, classes should not implement this interface directly rather IExportLocatorScope
    /// </summary>
    public interface ILocatorService
    {
        /// <summary>
        /// Can Locator type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="key">key to use while locating</param>
        /// <returns></returns>
        bool CanLocate(Type type, object key = null);

        /// <summary>
        /// Locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>located instance</returns>
        object Locate(Type type);

        /// <summary>
        /// Locate type
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <returns>located instance</returns>
        T Locate<T>();

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data to be used during construction</param>
        /// <param name="withKey">key to use for locating type</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        object Locate(Type type, object extraData = null, object withKey = null, bool isDynamic = false);

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <param name="extraData">extra data</param>
        /// <param name="withKey">key to use during construction</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        T Locate<T>(object extraData = null, object withKey = null, bool isDynamic = false);

        /// <summary>
        /// Locate all instances of a specific type
        /// </summary>
        /// <param name="type">type ot locate</param>
        /// <param name="extraData">extra data to be used while locating</param>
        /// <param name="filter">strategy filter</param>
        /// <param name="withKey">locate with key</param>
        /// <param name="comparer">comparer to use to sort collection</param>
        /// <returns></returns>
        List<object> LocateAll(Type type, object extraData = null, ExportStrategyFilter filter = null, object withKey = null, IComparer<object> comparer = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extraData"></param>
        /// <param name="filter"></param>
        /// <param name="withKey"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        List<T> LocateAll<T>(object extraData = null, ExportStrategyFilter filter = null, object withKey = null, IComparer<T> comparer = null);
        
        /// <summary>
        /// Try to locate an export by type
        /// </summary>
        /// <typeparam name="T">locate type</typeparam>
        /// <param name="value">out value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <returns></returns>
        bool TryLocate<T>(out T value, object extraData = null, ExportStrategyFilter consider = null, object withKey = null);

        /// <summary>
        /// try to locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="value">located value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <returns></returns>
        bool TryLocate(Type type, out object value, object extraData = null, ExportStrategyFilter consider = null, object withKey = null);

    }
}
