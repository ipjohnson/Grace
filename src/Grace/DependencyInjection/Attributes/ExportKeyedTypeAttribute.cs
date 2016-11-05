using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
    /// <summary>
    /// Adds a key to an exported class
    /// </summary>
    public class ExportKeyedTypeAttribute : Attribute, IExportKeyedTypeAttribute
    {
        private readonly Type _type;
        private readonly object _key;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">export type</param>
        /// <param name="key">export key</param>
        public ExportKeyedTypeAttribute(Type type, object key)
        {
            _type = type;
            _key = key;
        }

        public Tuple<Type, object> ProvideKey(Type attributedType)
        {
            return new Tuple<Type, object>(_type, _key);
        }
    }
}
