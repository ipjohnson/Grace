using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Attributes that implement this interface will be called during discovery to provide metadata for an attributed type
	/// </summary>
	public interface IExportMetadataAttribute
	{
		/// <summary>
		/// Provide the metadata for an attributed type
		/// </summary>
		/// <param name="attributedType">attributed type</param>
		/// <returns>a named piece of metadata</returns>
		IEnumerable<KeyValuePair<object, object>> ProvideMetadata(Type attributedType);
	}
}