using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{

    public class MemberInjectionInfo
    {
        public MemberInfo MemberInfo { get; set; }

        public Expression CreateExpression { get; set; }

        public bool IsRequired { get; set; }

        public object DefaultValue { get; set; }

        public bool IsDynamic { get; set; }
    }

    public interface IMemeberInjectionSelector
    {
        IEnumerable<MemberInjectionInfo> GetMembers(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request);
    }
}
