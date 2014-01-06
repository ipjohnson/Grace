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
	}
}