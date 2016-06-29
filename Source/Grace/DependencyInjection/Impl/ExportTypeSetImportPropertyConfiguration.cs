using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public partial class ExportTypeSetConfiguration
    {
        private class ImportGlobalPropertyInfo
        {
            public Type PropertyType { get; set; }

            public string PropertyName { get; set; }

            public bool IsRequired { get; set; }

            public object DefaultValue { get; set; }

            public bool AfterConstruction { get; set; }

            public Func<PropertyInfo, bool> PropertyFilter { get; set; }

            public ExportStrategyFilter Consider { get; set; }

            public IExportValueProvider ValueProvider { get; set; }
        }


        /// <summary>
        /// Import attributed members
        /// </summary>
        /// <returns></returns>
        public IExportTypeSetConfiguration ImportAttributedMembers()
        {
            _importAttributedMembers = true;

            return this;
        }

        /// <summary>
        /// Import members matching filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration ImportMembers(Func<MemberInfo, bool> filter)
        {
            importMembersList.Add(filter);

            return this;
        }

        /// <summary>
        /// Import properties of type TProperty and by name
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <returns>
        /// configuration object
        /// </returns>
        public IExportTypeSetImportPropertyConfiguration ImportProperty<TProperty>()
        {
            importPropertiesList.Add(new ImportGlobalPropertyInfo { PropertyType = typeof(TProperty), IsRequired = true });

            return this;
        }

        /// <summary>
        /// Import all properties that match the type
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public IExportTypeSetImportPropertyConfiguration ImportProperty(Type propertyType = null)
        {
            importPropertiesList.Add(new ImportGlobalPropertyInfo { PropertyType = propertyType, IsRequired = true });

            return this;
        }

        /// <summary>
        /// Property Name to import
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>
        /// configuration object
        /// </returns>
        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.Named(string propertyName)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].PropertyName = propertyName;
            }

            return this;
        }

        /// <summary>
        /// Is it required
        /// </summary>
        /// <param name="value">is required</param>
        /// <returns>
        /// configuration object
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.IsRequired(bool value)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].IsRequired = value;
            }

            return this;
        }

        /// <summary>
        /// Default Value if one can't be located
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.DefaultValue(object defaultValue)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].DefaultValue = defaultValue;
            }

            return this;
        }

        /// <summary>
        /// Apply delegate to choose export
        /// </summary>
        /// <param name="consider">consider filter</param>
        /// <returns>
        /// configuration object
        /// </returns>
        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.Consider(ExportStrategyFilter consider)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].Consider = consider;
            }

            return this;
        }

        /// <summary>
        /// Using Value provider
        /// </summary>
        /// <param name="activationDelegate"></param>
        /// <returns>
        /// configuration object
        /// </returns>
        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.UsingValue(ExportActivationDelegate activationDelegate)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].ValueProvider = new ExportActivationValueProvider(activationDelegate);
            }

            return this;
        }

        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.UsingValue(object value)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].ValueProvider = new ExportActivationValueProvider((scope,context) => value);
            }

            return this;
        }

        /// <summary>
        /// Use value provider
        /// </summary>
        /// <param name="valueProvider">value provider</param>
        /// <returns>
        /// configuration object
        /// </returns>
        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.UsingValueProvider(IExportValueProvider valueProvider)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].ValueProvider = valueProvider;
            }

            return this;
        }

        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.AfterConstruction()
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].AfterConstruction = true;
            }

            return this;
        }

        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.OnlyOn(Func<Type, bool> filter)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].PropertyFilter = p => filter(p.PropertyType);
            }

            return this;
        }

        IExportTypeSetImportPropertyConfiguration IExportTypeSetImportPropertyConfiguration.Matching(Func<PropertyInfo, bool> matchingFilter)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].PropertyFilter = matchingFilter;
            }

            return this;
        }
    }
}
