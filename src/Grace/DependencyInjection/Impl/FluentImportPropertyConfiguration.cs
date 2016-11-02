using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration class for importing a property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProp"></typeparam>
    public class FluentImportPropertyConfiguration<T, TProp> : WrapperFluentExportStrategyConfiguration<T>, IFluentImportPropertyConfiguration<T, TProp>
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
        public IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        #endregion

        
    }
}
