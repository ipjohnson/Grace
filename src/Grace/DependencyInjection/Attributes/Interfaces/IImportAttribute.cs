using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	/// <summary>
	/// Information about how the import should be performed
	/// </summary>
	public class ImportAttributeInfo
	{
        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue { get; set; }

		/// <summary>
		/// Is the import required
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		/// The key that should be used when importing
		/// </summary>
		public object ImportKey { get; set; }
        
		/// <summary>
		/// Import Filter 
		/// </summary>
		public ActivationStrategyFilter ExportStrategyFilter { get; set; }

		/// <summary>
		/// Comparer object for import
		/// </summary>
		public object Comparer { get; set; }
	}

	/// <summary>
	/// Attributes that implement this interface will be included while discovering attributes for importing
	/// </summary>
	public interface IImportAttribute
	{
		/// <summary>
		/// Provides information about the import
		/// </summary>
		/// <param name="attributedType">the type that is attributed, null when attributed on methods</param>
		/// <param name="attributedName">the name of the method, property, or parameter name</param>
		/// <returns></returns>
		ImportAttributeInfo ProvideImportInfo(Type attributedType, string attributedName);
	}
}