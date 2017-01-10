using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Attributes
{
    /// <summary>
    /// Limits an export to only be used when the importing member has a particular attribute
    /// </summary>
    public class WhenMemberHasAttribute : Attribute, IExportConditionAttribute
    {
        private readonly Type _attributeType;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="attributeType"></param>
        public WhenMemberHasAttribute(Type attributeType)
        {
            _attributeType = attributeType;
        }

        /// <summary>
        /// Provide an export condition for an attirbuted type
        /// </summary>
        /// <param name="attributedType"></param>
        /// <returns></returns>
        public ICompiledCondition ProvideCondition(Type attributedType)
        {
            return new WhenClassHas(_attributeType);
        }
    }
}