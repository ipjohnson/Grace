using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	public interface IFluentWithCtorConfiguration<TParam> : IFluentExportStrategyConfiguration
	{
		IFluentWithCtorConfiguration<TParam> IsRequired(bool isRequired = true);

		IFluentWithCtorConfiguration<TParam> Named(string name);

		IFluentWithCtorConfiguration<TParam> ImportName(string importName);

		IFluentWithCtorConfiguration<TParam> Consider(ExportStrategyFilter filter);
	}

	public interface IFluentWithCtorConfiguration<T, TParam> : IFluentExportStrategyConfiguration<T>
	{
		IFluentWithCtorConfiguration<T, TParam> IsRequired(bool isRequired = true);

		IFluentWithCtorConfiguration<T, TParam> Named(string name);

		IFluentWithCtorConfiguration<T, TParam> ImportName(string importName);

		IFluentWithCtorConfiguration<T, TParam> Consider(ExportStrategyFilter filter);
	}
}
