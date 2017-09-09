using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// injects fields and properties of a specific type
    /// </summary>
    public class PropertyFieldInjectionSelector : IMemberInjectionSelector
    {
        private readonly Type _memberType;
        private readonly Func<MemberInfo, bool> _filter;
        private readonly bool _processAttributes;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="memberType"></param>
        /// <param name="filter"></param>
        /// <param name="processAttributes"></param>
        public PropertyFieldInjectionSelector(Type memberType, Func<MemberInfo, bool> filter, bool processAttributes)
        {
            _memberType = memberType;
            _filter = filter;
            _processAttributes = processAttributes;
            IsRequired = true;
        }
        
        /// <summary>
        /// Key to use during locate
        /// </summary>
        public object LocateKey { get; set; }

        /// <summary>
        /// Is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Get a list of member injection info for a specific type
        /// </summary>
        /// <param name="type">type being activated</param>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="request">request</param>
        /// <returns>members being injected</returns>
        public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            foreach (var property in type.GetRuntimeProperties())
            {
                if (!property.CanWrite || !property.SetMethod.IsPublic || property.SetMethod.IsStatic)
                {
                    continue;
                }

                if (ReflectionService.CheckTypeIsBasedOnAnotherType(property.PropertyType, _memberType))
                {
                    if (_filter == null || _filter(property))
                    {
                        var importAttribute = _processAttributes ?
                            property.GetCustomAttributes().OfType<IImportAttribute>().FirstOrDefault() :
                            null;

                        var importInfo = importAttribute?.ProvideImportInfo(property.PropertyType,property.Name);

                        object key = importInfo?.ImportKey ?? LocateKey;

                        if (key == null &&
                            injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(property.PropertyType))
                        {
                            key = property.Name;
                        }

                        yield return new MemberInjectionInfo { MemberInfo = property, LocateKey = key, IsRequired = importInfo?.IsRequired ?? IsRequired };
                    }
                }
            }

            foreach (var field in type.GetRuntimeFields())
            {
                if (!field.IsPublic || field.IsStatic)
                {
                    continue;
                }
                if (ReflectionService.CheckTypeIsBasedOnAnotherType(field.FieldType, _memberType))
                {
                    if (_filter == null || _filter(field))
                    {
                        var importAttribute = _processAttributes ?
                            field.GetCustomAttributes().OfType<IImportAttribute>().FirstOrDefault() :
                            null;

                        var importInfo = importAttribute?.ProvideImportInfo(field.FieldType, field.Name);

                        object key = importInfo?.ImportKey ?? LocateKey;

                        if (key == null &&
                            injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(field.FieldType))
                        {
                            key = field.Name;
                        }

                        yield return new MemberInjectionInfo { MemberInfo = field, LocateKey = key, IsRequired = importInfo?.IsRequired ?? IsRequired };
                    }
                }
            }
        }

        /// <summary>
        /// Get Methods to inject
        /// </summary>
        /// <param name="type">type being activated</param>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="request">request</param>
        /// <returns>methods being injected</returns>
        public IEnumerable<MethodInjectionInfo> GetMethods(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            yield break;
        }
    }
}
