namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// This interface is used by the SingletonPerRequestContainer
	/// It is used to provide a per request Lifestyle container
	/// </summary>
	public interface IPerRequestLifestyleProvider
	{
		/// <summary>
		/// Called to provide a new per request Lifestyle container
		/// </summary>
		/// <returns>new Lifestyle container</returns>
		ILifestyle ProvideContainer();
	}
}