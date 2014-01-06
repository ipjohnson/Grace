using System;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
	/// <summary>
	/// 
	/// </summary>
	public class ImportAttributedWithAttribute : Attribute, IImportAttribute
	{
		private readonly Type attributeType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="attributeType"></param>
		public ImportAttributedWithAttribute(Type attributeType)
		{
			this.attributeType = attributeType;
		}

		/// <summary>
		/// Import name
		/// </summary>
		public string ImportName { get; set; }

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
				       ExportStrategyFilter = FilterMethod,
				       ImportName = ImportName,
				       IsRequired = Required
			       };
		}

		private bool FilterMethod(IInjectionContext context, IExportStrategy strategy)
		{
			Func<Attribute, bool> testMethod =
				attribute => attribute.GetType().GetTypeInfo().IsAssignableFrom(attributeType.GetTypeInfo());

			Attribute attr = strategy.Attributes.FirstOrDefault(testMethod);

			return attr != null;
		}
	}
}