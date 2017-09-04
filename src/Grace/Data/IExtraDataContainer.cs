using System.Collections.Generic;

namespace Grace.Data
{
	/// <summary>
	/// Used to store extra data, implemented by IInjectionContext and IExportLocator
	/// </summary>
	public interface IExtraDataContainer
	{
        /// <summary>
        /// Keys for data
        /// </summary>
        IEnumerable<object> Keys { get; }

        /// <summary>
        /// Values for data
        /// </summary>
        IEnumerable<object> Values { get; }

        /// <summary>
        /// Enumeration of all the key value pairs
        /// </summary>
        IEnumerable<KeyValuePair<object,object>> KeyValuePairs { get; }

	    /// <summary>
	    /// Extra data associated with the injection request. 
	    /// </summary>
	    /// <param name="key">key of the data object to get</param>
	    /// <returns>data value</returns>
	    object GetExtraData(object key);

	    /// <summary>
	    /// Sets extra data on the injection context
	    /// </summary>
	    /// <param name="key">object name</param>
	    /// <param name="newValue">new object value</param>
	    /// <param name="replaceIfExists">replace value if key exists</param>
	    /// <returns>the final value of key</returns>
	    object SetExtraData(object key, object newValue, bool replaceIfExists = true);
	}

    public static class ExtraDataContainerExtensions
    {
        public static T GetExtraData<T>(this IExtraDataContainer container,  object key)
        {
            return (T)container.GetExtraData(key);
        }
    }
}