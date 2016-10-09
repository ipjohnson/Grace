namespace Grace.DependencyInjection
{
    /// <summary>
    /// Implement this interface to package registration together
    /// </summary>
    public interface IConfigurationModule
    {
        /// <summary>
        /// Configure the block
        /// </summary>
        /// <param name="registrationBlock">registration block</param>
        void Configure(IExportRegistrationBlock registrationBlock);
    }
}
