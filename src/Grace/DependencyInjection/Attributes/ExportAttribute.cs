using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Attributes
{
    /// <summary>
    /// This attribute is used to mark a type for export. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportAttribute : Attribute, IExportAttribute
    {
        /// <summary>
        /// Export by type rather than by name
        /// </summary>
        /// <param name="exportTypes"></param>
        public ExportAttribute(params Type[] exportTypes)
        {
            if (exportTypes != null && exportTypes.Length > 0)
            {
                ExportTypes = exportTypes;
            }
        }


        /// <summary>
        /// List of export types
        /// </summary>
        protected Type[] ExportTypes { get; set; }

        /// <summary>
        /// Provide a list of types to export as
        /// </summary>
        /// <param name="attributedType"></param>
        /// <returns></returns>
        public IEnumerable<Type> ProvideExportTypes(Type attributedType)
        {
            if (ExportTypes != null)
            {
                return ExportTypes;
            }

            return new[] { attributedType };
        }
    }
}