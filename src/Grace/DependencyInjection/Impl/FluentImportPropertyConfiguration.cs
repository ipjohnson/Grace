using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Import property configuration
    /// </summary>
    public class FluentImportPropertyConfiguration : ProxyFluentExportStrategyConfiguration, IFluentImportPropertyConfiguration
    {
        private readonly MemberInjectionInfo _memberInjectionInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="memberInjectionInfo"></param>
        public FluentImportPropertyConfiguration(IFluentExportStrategyConfiguration strategy, MemberInjectionInfo memberInjectionInfo) : base(strategy)
        {
            _memberInjectionInfo = memberInjectionInfo;
        }

        /// <summary>
        /// use a filter delegate when importing property
        /// </summary>
        /// <param name="consider">filter delegate</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration Consider(ActivationStrategyFilter consider)
        {
            _memberInjectionInfo.Filter = consider;

            return this;
        }

        /// <summary>
        /// Default value if one can not be found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration DefaultValue(object defaultValue)
        {
            _memberInjectionInfo.DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        /// Is the property required
        /// </summary>
        /// <param name="isRequired">is required</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration IsRequired(bool isRequired = true)
        {
            _memberInjectionInfo.IsRequired = isRequired;

            return this;
        }

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">locate key</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration LocateWithKey(object locateKey)
        {
            _memberInjectionInfo.LocateKey = locateKey;

            return this;
        }

        /// <summary>
        /// Provide value to be used for property
        /// </summary>
        /// <param name="value">value to use for property</param>
        /// <returns></returns>
        public IFluentImportPropertyConfiguration Value(object value)
        {
            _memberInjectionInfo.CreateExpression = 
                Expression.Constant(value, ((PropertyInfo)_memberInjectionInfo.MemberInfo).PropertyType);

            return this;
        }
    }

    /// <summary>
    /// Configuration class for importing a property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProp"></typeparam>
    public class FluentImportPropertyConfiguration<T, TProp> : ProxyFluentExportStrategyConfiguration<T>, IFluentImportPropertyConfiguration<T, TProp>
    {
        private readonly MemberInjectionInfo _memberInjectionInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="memberInjectionInfo"></param>
        public FluentImportPropertyConfiguration(IFluentExportStrategyConfiguration<T> strategy, MemberInjectionInfo memberInjectionInfo) : base(strategy)
        {

            _memberInjectionInfo = memberInjectionInfo;
        }

        #region Property methods

        /// <summary>
        /// use a filter delegate when importing property
        /// </summary>
        /// <param name="consider">filter delegate</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration<T, TProp> Consider(ActivationStrategyFilter consider)
        {
            _memberInjectionInfo.Filter = consider;

            return this;
        }

        /// <summary>
        /// Default value if one can not be found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration<T, TProp> DefaultValue(TProp defaultValue)
        {
            _memberInjectionInfo.DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        /// Is the property required
        /// </summary>
        /// <param name="isRequired">is required</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true)
        {
            _memberInjectionInfo.IsRequired = isRequired;

            return this;
        }

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKey">locate key</param>
        /// <returns>configuration object</returns>
        public IFluentImportPropertyConfiguration<T, TProp> LocateWithKey(object locateKey)
        {
            _memberInjectionInfo.LocateKey = locateKey;

            return this;
        }

        /// <summary>
        /// Provide value for property
        /// </summary>
        /// <param name="value">property value</param>
        /// <returns></returns>
        public IFluentImportPropertyConfiguration<T, TProp> Value(TProp value)
        {
            _memberInjectionInfo.CreateExpression = Expression.Constant(value);

            return this;
        }

        #endregion


    }
}
