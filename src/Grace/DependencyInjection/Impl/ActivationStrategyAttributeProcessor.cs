﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Process attributes on activation strategy
    /// </summary>
    public interface IActivationStrategyAttributeProcessor
    {
        /// <summary>
        /// Process attribute on strategy
        /// </summary>
        /// <param name="strategy">activation strategy</param>
        void ProcessAttributeForConfigurableActivationStrategy(IConfigurableActivationStrategy strategy);
    }

    /// <summary>
    /// Process attributes on strategy
    /// </summary>
    public class ActivationStrategyAttributeProcessor : IActivationStrategyAttributeProcessor
    {
        /// <summary>
        /// Process attribute on strategy
        /// </summary>
        /// <param name="strategy">activation strategy</param>
        public void ProcessAttributeForConfigurableActivationStrategy(IConfigurableActivationStrategy strategy)
        {
            ProcessClassAttributes(strategy);

            ProcessFields(strategy);

            ProcessProperties(strategy);

            ProcessMethods(strategy);
        }

        private void ProcessMethods(IConfigurableActivationStrategy strategy)
        {
            foreach (var methodInfo in strategy.ActivationType.GetRuntimeMethods())
            {
                if (!methodInfo.IsPublic || methodInfo.IsStatic)
                {
                    continue;
                }

                foreach (var attribute in methodInfo.GetCustomAttributes())
                {
                    var importAttribute = attribute as IImportAttribute;

                    var importInfo = importAttribute?.ProvideImportInfo(strategy.ActivationType, methodInfo.Name);

                    if (importInfo != null)
                    {
                        strategy.MethodInjectionInfo(new MethodInjectionInfo { Method = methodInfo });
                    }
                }
            }
        }

        private void ProcessProperties(IConfigurableActivationStrategy strategy)
        {
            foreach (var propertyInfo in strategy.ActivationType.GetRuntimeProperties())
            {
                if (!propertyInfo.CanWrite ||
                    !propertyInfo.SetMethod.IsPublic ||
                     propertyInfo.SetMethod.IsStatic)
                {
                    continue;
                }

                foreach (var attribute in propertyInfo.GetCustomAttributes())
                {
                    var importAttr = attribute as IImportAttribute;

                    var importInfo = importAttr?.ProvideImportInfo(strategy.ActivationType, propertyInfo.Name);

                    if (importInfo != null)
                    {
                        var name = propertyInfo.Name;

                        strategy.MemberInjectionSelector(new PublicMemeberInjectionSelector(m => m.Name == name) { IsRequired = importInfo.IsRequired });
                    }
                }
            }
        }

        private void ProcessFields(IConfigurableActivationStrategy strategy)
        {
            foreach (var fieldInfo in strategy.ActivationType.GetRuntimeFields())
            {
                if (!fieldInfo.IsPublic || fieldInfo.IsStatic)
                {
                    continue;
                }

                foreach (var attribute in fieldInfo.GetCustomAttributes())
                {
                    var importAttr = attribute as IImportAttribute;

                    var importInfo = importAttr?.ProvideImportInfo(strategy.ActivationType, fieldInfo.Name);

                    if (importInfo != null)
                    {
                        var name = fieldInfo.Name;

                        strategy.MemberInjectionSelector(new PublicMemeberInjectionSelector(m => m.Name == name) { IsRequired = importInfo.IsRequired });
                    }
                }
            }
        }

        private void ProcessClassAttributes(IConfigurableActivationStrategy strategy)
        {
            foreach (var attribute in strategy.ActivationType.GetTypeInfo().GetCustomAttributes())
            {
                IExportAttribute exportAttribute = attribute as IExportAttribute;

                var types = exportAttribute?.ProvideExportTypes(strategy.ActivationType);

                if (types != null)
                {
                    foreach (var type in types)
                    {
                        strategy.AddExportAs(type);
                    }
                }

                IExportConditionAttribute conditionAttribute = attribute as IExportConditionAttribute;

                var condition = conditionAttribute?.ProvideCondition(strategy.ActivationType);

                if (condition != null)
                {
                    strategy.AddCondition(condition);
                }

                IExportKeyedTypeAttribute keyedTypeAttribute = attribute as IExportKeyedTypeAttribute;

                var tuple = keyedTypeAttribute?.ProvideKey(strategy.ActivationType);

                if (tuple != null)
                {
                    strategy.AddExportAsKeyed(tuple.Item1, tuple.Item2);
                }
            }
        }
    }
}