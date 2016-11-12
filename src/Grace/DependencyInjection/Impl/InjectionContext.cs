using System;
using System.Linq;
using System.Reflection;
using Grace.Data;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Injection context that can be created on the fly
    /// </summary>
    public class InjectionContext : IInjectionContext
    {
        private ImmutableHashTree<string, object> _extraDataProperties;
        private ImmutableHashTree<object, object> _extraDataValues = ImmutableHashTree<object, object>.Empty;

        /// <summary>
        /// private constructor for cloning
        /// </summary>
        private InjectionContext()
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="extraData">extra data</param>
        public InjectionContext(object extraData)
        {
            ExtraData = extraData;
            SharedData = new InjectionContextSharedData();

            _extraDataProperties = extraData != null ? 
                ReflectionService.GetPropertiesFromObject(extraData) : 
                ImmutableHashTree<string, object>.Empty;
        }

        /// <summary>
        /// Get data from context
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public object GetExtraData(object key)
        {
            var stringKey = key as string;

            if (stringKey != null)
            {
                return _extraDataProperties.GetValueOrDefault(stringKey) ??
                       _extraDataValues.GetValueOrDefault(key);
            }

            return _extraDataValues.GetValueOrDefault(key);
        }

        /// <summary>
        /// Set data into context
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="replaceIfExists"></param>
        public void SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            _extraDataValues = _extraDataValues.Add(key, newValue, (o, n) => replaceIfExists ? n : o);
        }

        /// <summary>
        /// Original object that was passed in as extra data when locate was called
        /// </summary>
        public object ExtraData { get; private set; }

        /// <summary>
        /// Data container that is shared between all context for an object graph
        /// </summary>
        public IInjectionContextSharedData SharedData { get; private set; }

        /// <summary>
        /// Get a value by type from the extra data
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>instance or null if one can't be found</returns>
        public object GetValueByType(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return _extraDataProperties.Values.FirstOrDefault(v => typeInfo.IsAssignableFrom(v.GetType().GetTypeInfo()));
        }

        /// <summary>
        /// Clone the extra data provider
        /// </summary>
        /// <returns></returns>
        public IInjectionContext Clone()
        {
            return new InjectionContext
            {
                _extraDataProperties = _extraDataProperties,
                _extraDataValues = _extraDataValues,
                SharedData = SharedData,
                ExtraData = ExtraData
            };
        }
    }
}
