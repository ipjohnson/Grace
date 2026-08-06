using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

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
                    var type = strategy.ActivationType;
                    if (ImportAttributeInfo.For(constructorInfo, type, type.Name) != null)
                    {
                        strategy.SelectedConstructor = constructorInfo;
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

                if (ImportAttributeInfo.For(methodInfo, null, methodInfo.Name) != null)
                {
                    var methodInjection = new MethodInjectionInfo { Method = methodInfo };
                    strategy.MethodInjectionInfo(methodInjection);

                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        var importInfo = ImportAttributeInfo.For(parameterInfo, strategy.ActivationType, parameterInfo.Name);
                        if (importInfo != null)
                        {
                            methodInjection.MethodParameterInfo(
                                new MethodParameterInfo(parameterInfo.Name)
                                {
                                    IsRequired = importInfo.IsRequired,
                                    LocateKey = importInfo.ImportKey,
                                });
                        }
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

                var importInfo = ImportAttributeInfo.For(propertyInfo, strategy.ActivationType, propertyInfo.Name);
                if (importInfo != null)
                {
                    var name = propertyInfo.Name;

                    strategy.MemberInjectionSelector(new PropertyFieldInjectionSelector(
                        propertyInfo.PropertyType, 
                        m => m.Name == name, 
                        false)
                    { 
                        IsRequired = importInfo.IsRequired, 
                        LocateKey = importInfo.ImportKey
                    });
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

                var importInfo = ImportAttributeInfo.For(fieldInfo, strategy.ActivationType, fieldInfo.Name);
                if (importInfo != null)
                {
                    var name = fieldInfo.Name;

                    strategy.MemberInjectionSelector(new PropertyFieldInjectionSelector(
                        fieldInfo.FieldType, 
                        info => info.Name == name, 
                        false) 
                    { 
                        IsRequired = importInfo.IsRequired, 
                        LocateKey = importInfo.ImportKey,
                    });
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
