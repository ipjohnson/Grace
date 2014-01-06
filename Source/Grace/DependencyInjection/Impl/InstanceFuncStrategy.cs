namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This strategy takes a func and calls it when ever
	/// </summary>
	/// <typeparam name="T">type of instance that will be exported</typeparam>
	public class InstanceFuncStrategy<T> : ConfigurableExportStrategy
	{
		private readonly ExportFunction<T> instanceFunc;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="instanceFunc">export func</param>
		public InstanceFuncStrategy(ExportFunction<T> instanceFunc)
			: base(typeof(T))
		{
			this.instanceFunc = instanceFunc;
		}

		/// <summary>
		/// Activate the strategy
		/// </summary>
		/// <param name="exportInjectionScope">export injection scope</param>
		/// <param name="context">injection context</param>
		/// <param name="consider">export strategy filter</param>
		/// <returns>export object</returns>
		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			if (lifestyle != null)
			{
				return lifestyle.Locate(InternalActivate, exportInjectionScope, context, this);
			}

			return InternalActivate(exportInjectionScope, context);
		}

		/// <summary>
		/// internal activate method
		/// </summary>
		/// <param name="injectionscope">injection scope</param>
		/// <param name="context">injection context</param>
		/// <returns>export instance</returns>
		private object InternalActivate(IInjectionScope injectionscope, IInjectionContext context)
		{
			object returnValue = instanceFunc(injectionscope, context);

			if (enrichWithDelegates != null)
			{
				foreach (EnrichWithDelegate enrichWithDelegate in enrichWithDelegates)
				{
					returnValue = enrichWithDelegate(injectionscope, context, returnValue);
				}
			}

			return returnValue;
		}
	}
}