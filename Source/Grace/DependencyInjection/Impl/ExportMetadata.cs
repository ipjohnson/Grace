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

		public object Key { get; private set; }
	}
}