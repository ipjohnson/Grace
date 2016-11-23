﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Information about a member that is being injected
    /// </summary>
    public class MemberInjectionInfo
    {
        /// <summary>
        /// Member info object
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// Expression that can be used to satify import
        /// </summary>
        public Expression CreateExpression { get; set; }

        /// <summary>
        /// Is it required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for member
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Should the locate be dynamic
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// key to use for locate
        /// </summary>
        public object LocateKey { get; set; }

        /// <summary>
        /// Filter to use for locate
        /// </summary>
        public ActivationStrategyFilter Filter { get; set; }
    }

    /// <summary>
    /// interface for selecting members that should be injected on a type
    /// </summary>
    public interface IMemeberInjectionSelector
    {
        /// <summary>
        /// Get a list of member injection info for a specific type
        /// </summary>
        /// <param name="type">type being activated</param>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="request">request</param>
        /// <returns>members being injected</returns>
        IEnumerable<MemberInjectionInfo> GetMembers(Type type, IInjectionScope injectionScope, IActivationExpressionRequest request);
    }
}
