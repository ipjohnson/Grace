namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Instance strategy represents the export strategy for a particular export instance
	/// </summary>
	public class InstanceStrategy<T> : ConfigurableExportStrategy
	{
		private readonly T instance;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="instance"></param>
		public InstanceStrategy(T instance) : base(typeof(T))
		{
			this.instance = instance;
		}

		/// <summary>
		/// Return the instance
		/// </summary>
		/// <param name="exportInjectionScope">injection scope</param>
		/// <param name="context">injection context</param>
		/// <param name="consider">export filter</param>
		/// <returns>export object</returns>
		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			return instance;
		}
	}
}