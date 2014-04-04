namespace Grace.DependencyInjection
{
	/// <summary>
	/// A collection of export filters that can convert to one ExportStrategyFilter
	/// </summary>
	public class ExportStrategyFilterGroup
	{
		private readonly ExportStrategyFilter[] filters;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="filters"></param>
		public ExportStrategyFilterGroup(params ExportStrategyFilter[] filters)
		{
			this.filters = filters;
		}

		/// <summary>
		/// Use Or logic instead of and logic
		/// </summary>
		public bool UseOr { get; set; }

		/// <summary>
		/// Converts export filter group to export strategy filter
		/// </summary>
		/// <param name="exportStrategyGroup"></param>
		/// <returns></returns>
		public static implicit operator ExportStrategyFilter(ExportStrategyFilterGroup exportStrategyGroup)
		{
			return exportStrategyGroup.InternalExportStrategyFilter;
		}

		/// <summary>
		/// Internal filter method that loops through the collection of filters
		/// </summary>
		/// <param name="context">injection context</param>
		/// <param name="strategy">export strategy</param>
		/// <returns>true if the strategy matches</returns>
		private bool InternalExportStrategyFilter(IInjectionContext context, IExportStrategy strategy)
		{
			if (UseOr)
			{
				foreach (ExportStrategyFilter exportStrategyFilter in filters)
				{
					if (exportStrategyFilter(context, strategy))
					{
						return true;
					}
				}

				return false;
			}

			foreach (ExportStrategyFilter exportStrategyFilter in filters)
			{
				if (!exportStrategyFilter(context, strategy))
				{
					return false;
				}
			}

			return true;
		}
	}
}