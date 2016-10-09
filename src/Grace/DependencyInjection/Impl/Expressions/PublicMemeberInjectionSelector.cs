using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public class PublicMemeberInjectionSelector : IMemeberInjectionSelector
    {
        private readonly Func<MemberInfo, bool> _picker;

        public PublicMemeberInjectionSelector(Func<MemberInfo, bool> picker)
        {
            _picker = picker;
        }

        public IEnumerable<MemberInjectionInfo> GetMembers(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
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
                    yield return new MemberInjectionInfo { MemberInfo = declaredMember };
                }
            }
        }
    }
}
