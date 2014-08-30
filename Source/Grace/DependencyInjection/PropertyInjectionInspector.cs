using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.Data;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Strategy Inspector that marks certain properties for injection
    /// Must be used before the strategy is initialized (i.e. can be added after the export is configured)
    /// </summary>
    public class PropertyInjectionInspector : IStrategyInspector
    {
        private readonly Type _propertyType;
        private readonly string _propertyName;
        private readonly Func<PropertyInfo,bool> _propertyPicker;
        private readonly IExportValueProvider _valueProvider;
        private readonly bool _afterConstructor;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="propertyType">property type to import, can be generic</param>
        /// <param name="propertyName">name of the property to inject, can be null</param>
        /// <param name="propertyPicker"></param>
        /// <param name="afterConstructor"></param>
        /// <param name="valueProvider"></param>
        public PropertyInjectionInspector(Type propertyType,
            string propertyName = null,
            Func<PropertyInfo,bool> propertyPicker = null,
            bool afterConstructor = false,
            IExportValueProvider valueProvider = null)
        {
            _propertyType = propertyType;
            _propertyName = propertyName;
            _propertyPicker = propertyPicker;
            _valueProvider = valueProvider;
            _afterConstructor = afterConstructor;
        }

        public void StrategyInitializing(IExportStrategy exportStrategy)
        {
            CompiledExportStrategy compiledExportStrategy = exportStrategy as CompiledExportStrategy;

            if (compiledExportStrategy != null)
            {
                foreach (PropertyInfo declaredProperty in exportStrategy.ActivationType.GetTypeInfo().DeclaredProperties)
                {
                    if (!declaredProperty.CanWrite ||
                        declaredProperty.SetMethod.IsStatic ||
                        !ReflectionService.CheckTypeIsBasedOnAnotherType(declaredProperty.PropertyType, _propertyType))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(_propertyName) &&
                        _propertyName != declaredProperty.Name)
                    {
                        continue;
                    }

                    if (_propertyPicker != null &&
                        !_propertyPicker(declaredProperty))
                    {
                        continue;
                    }

                    compiledExportStrategy.ImportProperty(new ImportPropertyInfo
                                                          {
                                                              AfterConstruction = _afterConstructor,
                                                              Property = declaredProperty,
                                                              ValueProvider = _valueProvider
                                                          });
                }
            }
        }
    }

    /// <summary>
    /// Strategy Inspector that marks certain properties for injection
    /// Must be used before the strategy is initialized (i.e. can be added after the export is configured)
    /// </summary>
    public class PropertyInjectionInspector<TProp> : PropertyInjectionInspector
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyPicker"></param>
        /// <param name="afterConstructor"></param>
        /// <param name="valueProvider"></param>
        public PropertyInjectionInspector( string propertyName = null, Func<PropertyInfo,bool> propertyPicker = null, bool afterConstructor = false, IExportValueProvider valueProvider = null) : 
            base(typeof(TProp),  propertyName,  propertyPicker, afterConstructor, valueProvider)
        {
        }
    }
}
