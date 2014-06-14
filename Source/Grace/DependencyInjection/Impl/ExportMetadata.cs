using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Export metadata 
	/// </summary>
	public class ExportMetadata : ReadOnlyDictionary<string, object>, IExportMetadata
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dictionary"></param>
		public ExportMetadata(object key, IDictionary<string, object> dictionary) : base(dictionary)
		{
			Key = key;
		}

		/// <summary>
		/// Key the export was registered with
		/// </summary>
		public object Key { get; private set; }

	    /// <summary>
	    /// Tests to see if metadata values match
	    /// </summary>
	    /// <param name="metadataName"></param>
	    /// <param name="metadataValue"></param>
	    /// <returns></returns>
	    public bool MetadataMatches(string metadataName, object metadataValue)
	    {
	        object testValue;

            if(metadataValue != null)
	        {
                return TryGetValue(metadataName, out testValue) && 
                       metadataValue.Equals(testValue);
            }

	        return TryGetValue(metadataName, out testValue) &&
	               testValue == null;
	    }
	}
}