namespace Grace.Data
{
	/// <summary>
	/// Used to store extra data, implemented by IInjectionContext and IExportLocator
	/// </summary>
	public interface IExtraDataContainer
	{
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
	    /// <param name="replaceIfExists"></param>
	    void SetExtraData(object key, object newValue, bool replaceIfExists = true);
	}
}