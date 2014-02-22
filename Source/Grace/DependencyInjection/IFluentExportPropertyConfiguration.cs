using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection
{
	public interface IFluentExportPropertyConfiguration : IFluentExportStrategyConfiguration
	{
		IFluentExportPropertyConfiguration WithName(string exportName);

		IFluentExportPropertyConfiguration WithType(Type exportType);

		IFluentExportPropertyConfiguration WithCondition(IExportCondition exportCondition);
	}

	public interface IFluentExportPropertyConfiguration<T, TProp> : IFluentExportStrategyConfiguration<T>
	{
		IFluentExportPropertyConfiguration<T, TProp> WithName(string exportName);

		IFluentExportPropertyConfiguration<T, TProp> WithType(Type exportType);

		IFluentExportPropertyConfiguration<T, TProp> WithCondition(IExportCondition exportCondition);
	}
}