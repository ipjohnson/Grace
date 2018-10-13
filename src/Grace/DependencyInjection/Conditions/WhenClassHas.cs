using System;
using System.Linq;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// condition class for testing if target has a particular attribute
    /// </summary>
    public class WhenClassHas : ICompiledCondition
    {
        private readonly Type _attributeType;
        private readonly Func<Attribute, bool> _filter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="filter"></param>
        public WhenClassHas(Type attributeType, Func<Attribute, bool> filter = null)
        {
            _attributeType = attributeType;
            _filter = filter;
        }
        
        /// <summary>
        /// Test if strategy meets condition
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>meets condition</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            var targetInfo = staticInjectionContext.InjectionStack.FirstOrDefault(
                info => info.RequestingStrategy?.StrategyType == ActivationStrategyType.ExportStrategy);

            if (targetInfo != null)
            {
                foreach (var attribute in targetInfo.InjectionTypeAttributes)
                {
                    if (attribute.GetType() == _attributeType && (_filter == null || _filter(attribute)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }


    /// <summary>
    /// condition class for testing if target has a particular attribute
    /// </summary>
    /// <typeparam name="TAttribute">attribute type</typeparam>
    public class WhenClassHas<TAttribute> : ICompiledCondition where TAttribute : Attribute
    {
        private readonly Func<TAttribute, bool> _filter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filter"></param>
        public WhenClassHas(Func<TAttribute, bool> filter = null)
        {
            _filter = filter;
        }

        /// <summary>
        /// Should the condition be run at expression creation time or every time a request is made for the type
        /// </summary>
        public bool IsRequestTimeCondition { get; } = false;

        /// <summary>
        /// If it is a request time condition does it need an injection context
        /// </summary>
        public bool RequiresInjectionContext { get; } = false;

        /// <summary>
        /// Test if strategy meets condition
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>meets condition</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            var targetInfo = staticInjectionContext.TargetInfo;

            if (targetInfo != null)
            {
                foreach (var attribute in targetInfo.InjectionTypeAttributes)
                {
                    var attrT = attribute as TAttribute;

                    if (attrT != null && (_filter == null || _filter(attrT)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
