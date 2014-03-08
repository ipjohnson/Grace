namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Export element collection
	/// </summary>
	public class ExportElementCollection : BaseElementCollection<ExportElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExportElementCollection() : base("export")
		{
		}
	}
}