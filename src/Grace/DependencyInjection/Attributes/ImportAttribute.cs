using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// This attribute can be used to attribute properties, methods, constructors or parameters
	///
	/// When applied to method or constructor 
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Parameter)]
	public class ImportAttribute : Attribute, IImportAttribute
	{
		/// <summary>
		/// default constructor
		/// </summary>
		public ImportAttribute()
		{
			Required = true;
		}
        
		/// <summary>
		/// Key to use when importing 
		/// </summary>
		public object Key { get; set; }

		/// <summary>
		/// Is this import required. True by default
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// Provides information about the import
		/// </summary>
		/// <param name="attributedType">the type that is attributed, null when attributed on methods</param>
		/// <param name="attributedName">the name of the method, property, or parameter name</param>
		/// <returns></returns>
		public ImportAttributeInfo ProvideImportInfo(Type attributedType, string attributedName)
		{
			return new ImportAttributeInfo
			       {
				       ImportKey = Key,
				       IsRequired = Required
			       };
		}
	}
}