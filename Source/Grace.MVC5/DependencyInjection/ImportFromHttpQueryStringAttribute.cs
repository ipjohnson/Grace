using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.MVC.DependencyInjection
{
	public class ImportFromHttpQueryStringAttribute : Attribute, IImportAttribute
	{
		public ImportFromHttpQueryStringAttribute()
		{
			Required = true;
		}

		public string ParameterName { get; set; }

		public bool Required { get; set; }

		public ImportAttributeInfo ProvideImportInfo(Type attributedType, string attributedName)
		{
			return new ImportAttributeInfo
			       {
				       IsRequired = Required,
				       ImportName = null,
				       ValueProvider = new HttpQueryStringProvider(ParameterName ?? attributedName)
			       };
		}
	}
}