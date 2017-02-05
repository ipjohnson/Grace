using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Selects properties that should be injected
    /// </summary>
    public class KnownMemberInjectionSelector : IMemberInjectionSelector
    {
        private readonly MemberInjectionInfo _memberInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="memberInfo"></param>
        public KnownMemberInjectionSelector(MemberInjectionInfo memberInfo)
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
        public IEnumerable<MemberInjectionInfo> GetPropertiesAndFields(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request)
        {
            yield return _memberInfo;
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
