using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Selects properties that should be injected
    /// </summary>
    public class PropertyMemberInjectionSelector : IMemeberInjectionSelector
    {
        private readonly MemberInjectionInfo _memberInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="memberInfo"></param>
        public PropertyMemberInjectionSelector(MemberInjectionInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        /// <summary>
        /// Get members to inject
        /// </summary>
        /// <param name="type"></param>
        /// <param name="injectionScope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<MemberInjectionInfo> GetMembers(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            yield return _memberInfo;
        }
    }
}
