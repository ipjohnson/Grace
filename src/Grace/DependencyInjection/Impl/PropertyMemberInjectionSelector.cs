using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    public class PropertyMemberInjectionSelector : IMemeberInjectionSelector
    {
        private readonly MemberInjectionInfo _memberInfo;

        public PropertyMemberInjectionSelector(MemberInjectionInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }


        public IEnumerable<MemberInjectionInfo> GetMembers(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            yield return _memberInfo;
        }
    }
}
