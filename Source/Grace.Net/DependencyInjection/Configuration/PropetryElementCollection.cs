namespace Grace.DependencyInjection.Configuration
{
	/// <summary>
	/// Collection of properties to apply to module
	/// </summary>
	public class PropetryElementCollection : BaseElementCollection<PropetryElement>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public PropetryElementCollection() : base("property")
		{
		}
	}
}