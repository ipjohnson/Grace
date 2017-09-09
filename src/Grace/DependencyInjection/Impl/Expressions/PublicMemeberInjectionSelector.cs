using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Select public members that can be injected
    /// </summary>
    public class PublicMemeberInjectionSelector : IMemberInjectionSelector
    {
        private readonly Func<MemberInfo, bool> _picker;
        private readonly bool _injectMethods;
        private readonly bool _processAttributes;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="picker"></param>
        /// <param name="injectMethods"></param>
        /// <param name="processAttributes"></param>
        public PublicMemeberInjectionSelector(Func<MemberInfo, bool> picker, bool injectMethods, bool processAttributes)
        {
            _picker = picker;
            _injectMethods = injectMethods;
            _processAttributes = processAttributes;
        }

        /// <summary>
        /// Is the member required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for member
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Get a list of member injection info for a specific type
        /// </summary>
        /// <param name="type">type being activated</param>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="request">request</param>
        /// <returns>members being injected</returns>
        public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            while (type != typeof(object))
            {
                foreach (var declaredMember in type.GetTypeInfo().DeclaredMembers)
                {
                    var propertyInfo = declaredMember as PropertyInfo;
                    Type importType = null;
                    
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.CanWrite && propertyInfo.SetMethod.IsPublic && !propertyInfo.SetMethod.IsStatic)
                        {
                            importType = propertyInfo.PropertyType;
                        }
                    }
                    else if (declaredMember is FieldInfo)
                    {
                        var fieldInfo = (FieldInfo) declaredMember;

                        if (fieldInfo.IsPublic && !fieldInfo.IsStatic)
                        {
                            importType = fieldInfo.FieldType;
                        }
                    }

                    if (importType != null && (_picker == null || _picker(declaredMember)))
                    {
                        var importAttribute = _processAttributes ? 
                                declaredMember.GetCustomAttributes().OfType<IImportAttribute>().FirstOrDefault() : 
                                null;

                        var importInfo = importAttribute?.ProvideImportInfo(importType, declaredMember.Name);

                        object key = importInfo?.ImportKey;

                        if (key == null && 
                            injectionScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(importType))
                        {
                            key = declaredMember.Name;
                        }

                        yield return new MemberInjectionInfo
                        {
                            MemberInfo = declaredMember,
                            IsRequired = importInfo?.IsRequired ?? IsRequired,
                            DefaultValue = DefaultValue,
                            LocateKey = key,
                        };
                    }
                }
                
                type = type.GetTypeInfo().BaseType;
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
            if (_injectMethods)
            {
                while (type != typeof(object))
                {
                    foreach (var declaredMember in type.GetTypeInfo().DeclaredMembers)
                    {
                        if (declaredMember is MethodInfo methodInfo &&
                            methodInfo.IsPublic &&
                            !methodInfo.IsStatic &&
                            (_picker == null ||
                             _picker(declaredMember)))
                        {
                            yield return new MethodInjectionInfo {Method = methodInfo};
                        }
                    }

                    type = type.GetTypeInfo().BaseType;
                }
            }
        }
    }
}
