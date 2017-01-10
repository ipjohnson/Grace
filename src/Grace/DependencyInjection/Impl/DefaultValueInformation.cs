namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Default value information to be used for during import
    /// </summary>
    public class DefaultValueInformation : IDefaultValueInformation
    {
        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue { get; set; }
    }
}
