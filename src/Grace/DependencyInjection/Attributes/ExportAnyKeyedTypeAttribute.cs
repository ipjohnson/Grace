using System;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
    /// <summary>
    /// Adds the special ImportKey.Any key to an exported class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportAnyKeyedTypeAttribute : Attribute, IExportKeyedTypeAttribute
    {
        private readonly Type _type;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">export type</param>
        public ExportAnyKeyedTypeAttribute(Type type)
        {
            _type = type;
        }

        /// <summary>
        /// Provide type and key for export
        /// </summary>
        /// <param name="attributedType"></param>
        public Tuple<Type, object> ProvideKey(Type attributedType)
        {
            return new Tuple<Type, object>(_type, ImportKey.Any);
        }
    }
}
