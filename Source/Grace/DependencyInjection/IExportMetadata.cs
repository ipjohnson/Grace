using System.Collections.Generic;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// 
	/// </summary>
	public interface IExportMetadata : IReadOnlyDictionary<string, object>
	{
		/// <summary>
		/// Key that the export was registered with
		/// </summary>
		object Key { get; }

        /// <summary>
        /// Tests to see if metadata values match
        /// </summary>
        /// <param name="metadataName"></param>
        /// <param name="metadataValue"></param>
        /// <returns></returns>
	    bool MetadataMatches(string metadataName, object metadataValue);
	}
}