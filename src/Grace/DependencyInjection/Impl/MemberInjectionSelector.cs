using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.Data;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class MemberInjectionSelector : IMemberInjectionSelector
    {
        private Type _memberType;
        private Func<MemberInfo, bool> _filter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="memberType"></param>
        /// <param name="filter"></param>
        public MemberInjectionSelector(Type memberType, Func<MemberInfo, bool> filter)
        {
            _memberType = memberType;
            _filter = filter;
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
                        yield return new MemberInjectionInfo { MemberInfo = property, LocateKey = LocateKey, IsRequired = IsRequired };
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
                        yield return new MemberInjectionInfo { MemberInfo = field, LocateKey = LocateKey, IsRequired = IsRequired };
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
