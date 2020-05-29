using System.Reflection;
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

            ProcessConstructors(strategy);
        }

        private void ProcessConstructors(IConfigurableActivationStrategy strategy)
        {
            foreach (var constructorInfo in strategy.ActivationType.GetTypeInfo().DeclaredConstructors)
            {
                if (constructorInfo.IsPublic)
                {
                    foreach (var customAttribute in constructorInfo.GetCustomAttributes())
                    {
                        if (customAttribute is IImportAttribute importAttribute)
                        {
                            var importInfo = importAttribute.ProvideImportInfo(strategy.ActivationType,
                                strategy.ActivationType.Name);

                            if (importInfo != null)
                            {
                                strategy.SelectedConstructor = constructorInfo;

                                break;
                            }
                        }
                    }
                }
            }
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

                        strategy.MemberInjectionSelector(new PropertyFieldInjectionSelector(propertyInfo.PropertyType, m => m.Name == name, false) { IsRequired = importInfo.IsRequired, LocateKey = importInfo.ImportKey });
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

                        strategy.MemberInjectionSelector(new PropertyFieldInjectionSelector(fieldInfo.FieldType, info => info.Name == name, false) { IsRequired = importInfo.IsRequired, LocateKey = importInfo.ImportKey });
                    }
                }
            }
        }

        private void ProcessClassAttributes(IConfigurableActivationStrategy strategy)
        {
            foreach (var attribute in strategy.ActivationType.GetTypeInfo().GetCustomAttributes())
            {
                var exportAttribute = attribute as IExportAttribute;

                var types = exportAttribute?.ProvideExportTypes(strategy.ActivationType);

                if (types != null)
                {
                    foreach (var type in types)
                    {
                        strategy.AddExportAs(type);
                    }
                }

                var conditionAttribute = attribute as IExportConditionAttribute;

                var condition = conditionAttribute?.ProvideCondition(strategy.ActivationType);

                if (condition != null)
                {
                    strategy.AddCondition(condition);
                }

                var keyedTypeAttribute = attribute as IExportKeyedTypeAttribute;

                var tuple = keyedTypeAttribute?.ProvideKey(strategy.ActivationType);

                if (tuple != null)
                {
                    strategy.AddExportAsKeyed(tuple.Item1, tuple.Item2);
                }

                if (attribute is IExportPriorityAttribute priorityAttribute)
                {
                    strategy.Priority = priorityAttribute.ProvidePriority(strategy.ActivationType);
                }
            }
        }
    }
}
