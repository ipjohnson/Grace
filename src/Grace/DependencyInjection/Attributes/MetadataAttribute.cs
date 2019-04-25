using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
    /// <summary>
    /// Attribute used to add metadata for attributed export
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MetadataAttribute : Attribute, IExportMetadataAttribute
    {
        private readonly object _key;
        private readonly object _value;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MetadataAttribute(object key, object value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Provide the metadata for an attributed type
        /// </summary>
        /// <param name="attributedType">attributed type</param>
        /// <returns>a named piece of metadata</returns>
        public IEnumerable<KeyValuePair<object, object>> ProvideMetadata(Type attributedType)
        {
            yield return new KeyValuePair<object, object>(_key, _value);
        }
    }
}
