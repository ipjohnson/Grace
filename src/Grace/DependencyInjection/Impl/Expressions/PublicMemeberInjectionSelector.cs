using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Select public members that can be injected
    /// </summary>
    public class PublicMemeberInjectionSelector : IMemeberInjectionSelector
    {
        private readonly Func<MemberInfo, bool> _picker;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="picker"></param>
        public PublicMemeberInjectionSelector(Func<MemberInfo, bool> picker)
        {
            _picker = picker;
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
            foreach (var declaredMember in type.GetTypeInfo().DeclaredMembers)
            {
                var propertyInfo = declaredMember as PropertyInfo;
                var test = false;

                if (propertyInfo != null)
                {
                    if (propertyInfo.CanWrite &&
                        propertyInfo.SetMethod.IsPublic &&
                       !propertyInfo.SetMethod.IsStatic)
                    {
                        test = true;
                    }
                }
                else if (declaredMember is FieldInfo)
                {
                    var fieldInfo = (FieldInfo)declaredMember;

                    if (fieldInfo.IsPublic && !fieldInfo.IsStatic)
                    {
                        test = true;
                    }
                }


                if (test && _picker(declaredMember))
                {
                    yield return new MemberInjectionInfo { MemberInfo = declaredMember, IsRequired = IsRequired, DefaultValue = DefaultValue };
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
            foreach (var declaredMember in type.GetTypeInfo().DeclaredMembers)
            {
                var methodInfo = declaredMember as MethodInfo;

                if (methodInfo != null && _picker(declaredMember))
                {
                    yield return new MethodInjectionInfo { Method = methodInfo };
                }
            }
        }
    }
}
