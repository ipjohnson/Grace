using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	public interface IFluentImportPropertyConfiguration : IFluentExportStrategyConfiguration
	{
		IFluentImportPropertyConfiguration IsRequired(bool isRequired = true);

		IFluentImportPropertyConfiguration Consider(ExportStrategyFilter consider);

		IFluentImportPropertyConfiguration UsingValue(Func<object> valueFunc);

		IFluentImportPropertyConfiguration UsingValueProvider(IExportValueProvider provider);
	}

	public interface IFluentImportPropertyConfiguration<T,TProp> : IFluentExportStrategyConfiguration<T>
	{
		IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true);

		IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider);

		IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<TProp> valueFunc);

		IFluentImportPropertyConfiguration<T, TProp> UsingValueProvider(IExportValueProvider provider);
	}
}
