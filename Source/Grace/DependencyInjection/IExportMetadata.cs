using System.Collections.Generic;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Provides a threadsafe metadata container for exports
	/// </summary>
	public interface IExportMetadata : IReadOnlyDictionary<string, object>
	{
		/// <summary>
		/// Key that the export was registered with
		/// </summary>
		object Key { get; }

        /// <summary>
        /// Returns true if there is no metadata.
        /// Note: this method is recommended over if(Count > 0)
        /// </summary>
        bool IsEmpty { get;  }

        /// <summary>
        /// Tests to see if metadata values match
        /// </summary>
        /// <param name="metadataName"></param>
        /// <param name="metadataValue"></param>
        /// <returns></returns>
	    bool MetadataMatches(string metadataName, object metadataValue);

        /// <summary>
        /// Add or update a metadata value
        /// </summary>
        /// <param name="metadataName">metadata value name</param>
        /// <param name="metadataValue">metadata value</param>
	    void AddOrUpdate(string metadataName, object metadataValue);
	}
}