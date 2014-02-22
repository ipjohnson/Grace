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
		/// <param name="dataName">name of data object to get</param>
		/// <returns>data value</returns>
		object GetExtraData(string dataName);

		/// <summary>
		/// Sets extra data on the injection context
		/// </summary>
		/// <param name="dataName">object name</param>
		/// <param name="newValue">new object value</param>
		void SetExtraData(string dataName, object newValue);
	}
}