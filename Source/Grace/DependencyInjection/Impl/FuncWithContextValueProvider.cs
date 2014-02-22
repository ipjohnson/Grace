using System;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// A value provider that calls a func to generate it's export
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FuncWithContextValueProvider<T> : IExportValueProvider
	{
		private readonly Func<IInjectionContext, T> valueFunc;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="valueFunc"></param>
		public FuncWithContextValueProvider(Func<IInjectionContext, T> valueFunc)
		{
			this.valueFunc = valueFunc;
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider)
		{
			return valueFunc(context);
		}
	}
}