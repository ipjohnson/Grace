using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.MVC.DependencyInjection
{
	public class ImportFromRequestPathAttribute : Attribute, IImportAttribute
	{
		public ImportFromRequestPathAttribute()
		{
			Required = true;
		}

		public bool Required { get; set; }

		public ImportAttributeInfo ProvideImportInfo(Type attributedType, string attributedName)
		{
			return new ImportAttributeInfo
			       {
				       IsRequired = Required,
				       ImportName = null,
				       ValueProvider = new HttpRequestPathProvider()
			       };
		}
	}
}