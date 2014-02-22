using System;

namespace Grace.DependencyInjection.Attributes.Interfaces
{
	public interface IImportFilterAttribute
	{
		ExportStrategyFilter ProvideFilter(Type attributedType, string attributedName);
	}
}