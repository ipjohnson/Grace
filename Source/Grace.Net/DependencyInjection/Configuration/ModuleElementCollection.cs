namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// List of modules to load
	/// </summary>
	public class ModuleElementCollection : BaseElementCollection<ModuleElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ModuleElementCollection() : base("module")
		{
		}
	}
}