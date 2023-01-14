namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Interface for providing singleton per lifestyle provide
    /// </summary>
    public interface IPerRequestLifestyleProvider
    {
        /// <summary>
        /// Provide container
        /// </summary>
        ICompiledLifestyle ProvideLifestyle();
    }
}
