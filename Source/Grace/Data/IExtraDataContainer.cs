namespace Grace.Data
{
	/// <summary>
	/// 
	/// </summary>
	public interface IExtraDataContainer
	{
		/// <summary>
		/// Extra data associated with the injection request. 
		/// </summary>
		/// <param name="dataName"></param>
		/// <returns></returns>
		object GetExtraData(string dataName);

		/// <summary>
		/// Sets extra data on the injection context
		/// </summary>
		/// <param name="dataName"></param>
		/// <param name="newValue"></param>
		void SetExtraData(string dataName, object newValue);
	}
}