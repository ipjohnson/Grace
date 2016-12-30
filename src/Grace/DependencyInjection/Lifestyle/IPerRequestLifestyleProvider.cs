namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Interface for providing singleton per lifestyle provide
    /// </summary>
    public interface IPerRequestLifestyleProvider
    {
        /// <summary>
        /// Provide contianer
        /// </summary>
        /// <returns></returns>
        ICompiledLifestyle ProvideLifestyle();
    }
}
